// js/trainer.js

// Global Data
let classes = [];
let students = [];
let attendanceRecords = [];
let exams = [];
let events = [];
let currentTrainerProfile = null;

// LocalStorage keys
const STORAGE_KEYS = {
    CLASSES: 'trainer_classes',
    STUDENTS: 'trainer_students',
    EXAMS: 'trainer_exams',
    EVENTS: 'trainer_events',
    PROFILE: 'trainer_profile',
    CURRENT_SECTION: 'trainer_current_section'
};

// Save data to localStorage
function saveToStorage(key, data) {
    try {
        localStorage.setItem(key, JSON.stringify(data));
    } catch (e) {
        console.error('Error saving to localStorage:', e);
    }
}

// Load data from localStorage
function loadFromStorage(key, defaultValue = null) {
    try {
        const data = localStorage.getItem(key);
        return data ? JSON.parse(data) : defaultValue;
    } catch (e) {
        console.error('Error loading from localStorage:', e);
        return defaultValue;
    }
}

// Clear all trainer data from localStorage
function clearTrainerStorage() {
    Object.values(STORAGE_KEYS).forEach(key => {
        localStorage.removeItem(key);
    });
}

// Initialization
document.addEventListener('DOMContentLoaded', async () => {
    // Check if user is logged in and is a trainer
    if (!checkAuth(['HuanLuyenVien', 'Trainer'])) return;

    // Restore current section from localStorage
    const savedSection = loadFromStorage(STORAGE_KEYS.CURRENT_SECTION, 'dashboard');
    showSection(savedSection);

    // Load initial data
    await renderAll();

    // Setup event listeners for modal close buttons
    const btnCloses = document.querySelectorAll('.btn-close');
    btnCloses.forEach(btn => btn.addEventListener('click', closeModal));

    // Setup logout
    const logoutBtn = document.getElementById('logoutGlobalBtn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', () => {
            if (confirm('Thoát khỏi hệ thống?')) {
                clearTrainerStorage(); // Clear local storage on logout
                logout();
            }
        });
    }
});

// Helper for finding items
function getClassById(id) { return classes.find(c => c.maLop === id); }

// Master Render Function
async function renderAll() {
    await fetchDashboardStats();
    await fetchClasses();
    await fetchStudents();
    await fetchExams();
    await fetchEvents();
    await fetchProfile();

    renderClassList();
    renderStudentTable();
    renderClassSelects();
    renderExamList();
    renderEventList();
    updateRecentActivities();

    if (document.getElementById("attendanceSheet") && document.getElementById("attClassSelect").value) {
        loadAttendanceSheet();
    }
}

// -----------------------------------------
// DASHBOARD
// -----------------------------------------
async function fetchDashboardStats() {
    try {
        const result = await apiCall('/trainer/dashboard');
        if (result.success && result.data && result.data.success) {
            const data = result.data.data;
            document.querySelector('#dashboard .text-primary').innerText = data.soLopDangDay < 10 ? '0' + data.soLopDangDay : data.soLopDangDay;
            document.querySelector('#dashboard .text-warning').innerText = data.tongHocVien;
            document.querySelector('#dashboard .text-success').innerText = data.tyLeChuyenCan + '%';
            document.querySelector('#dashboard .text-info').innerText = data.kyThiSapToi;
            saveToStorage('trainer_dashboard_stats', data);
        }
    } catch (e) {
        console.error("Lỗi lấy dashboard", e);
        // Try to load from localStorage if API fails
        const cachedStats = loadFromStorage('trainer_dashboard_stats');
        if (cachedStats) {
            document.querySelector('#dashboard .text-primary').innerText = cachedStats.soLopDangDay < 10 ? '0' + cachedStats.soLopDangDay : cachedStats.soLopDangDay;
            document.querySelector('#dashboard .text-warning').innerText = cachedStats.tongHocVien;
            document.querySelector('#dashboard .text-success').innerText = cachedStats.tyLeChuyenCan + '%';
            document.querySelector('#dashboard .text-info').innerText = cachedStats.kyThiSapToi;
        }
    }
}

let chart;
function updateChart() {
    const ctx = document.getElementById("attChart");
    if (!ctx) return;

    // Try to get real attendance data from API
    fetchAttendanceChartData().then(data => {
        if (data && data.labels && data.rates) {
            if (chart) chart.destroy();
            chart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: data.labels,
                    datasets: [{
                        label: 'Tỉ lệ chuyên cần %',
                        data: data.rates,
                        borderColor: '#C0392B',
                        backgroundColor: 'rgba(192,57,43,0.1)',
                        tension: 0.2,
                        fill: true
                    }]
                }
            });
        } else {
            // Fallback to dummy data if API doesn't return data
            let lastWeek = ["10/5", "11/5", "12/5", "13/5", "14/5", "15/5", "16/5"];
            let rates = [82, 85, 79, 88, 90, 85, 87];

            if (chart) chart.destroy();
            chart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: lastWeek,
                    datasets: [{
                        label: 'Tỉ lệ chuyên cần %',
                        data: rates,
                        borderColor: '#C0392B',
                        backgroundColor: 'rgba(192,57,43,0.1)',
                        tension: 0.2,
                        fill: true
                    }]
                }
            });
        }
    });
}

async function fetchAttendanceChartData() {
    try {
        const result = await apiCall('/trainer/attendance-chart');
        if (result.success && result.data.success) {
            return result.data.data;
        }
    } catch (e) {
        console.error("Lỗi lấy dữ liệu biểu đồ điểm danh", e);
    }
    return null;
}

function updateRecentActivities() {
    // Just some hardcoded activities for now since the API might not support an activity stream yet
    let acts = [
        "📌 Chào mừng huấn luyện viên quay trở lại!",
        "📌 Đừng quên điểm danh học viên mỗi buổi học.",
        "📌 Hệ thống đang hoạt động ổn định."
    ];
    document.getElementById("recentActivities").innerHTML = acts.map(a => `<li class="mb-2">🔹 ${a}</li>`).join("");
    document.getElementById("sysNotifications").innerHTML = `<i class="fas fa-bullhorn text-warning"></i> Nhắc nhở: Luôn theo dõi thông báo mới nhất từ câu lạc bộ.`;
    document.getElementById("notifCount").innerText = events.length;
}

// -----------------------------------------
// CLASS CRUD (View only, with edit and unregister)
// -----------------------------------------
async function fetchClasses() {
    try {
        const result = await apiCall('/trainer/classes');
        if (result.success && result.data && result.data.success) {
            classes = result.data.data;
            saveToStorage(STORAGE_KEYS.CLASSES, classes);
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách lớp", e);
        // Load from localStorage if API fails
        classes = loadFromStorage(STORAGE_KEYS.CLASSES, []);
    }
}

function renderClassList() {
    let html = "";
    if (classes.length === 0) {
        html = `<tr><td colspan="6" class="text-center">Chưa có lớp học nào</td></tr>`;
    } else {
        classes.forEach(c => {
            html += `<tr>
                <td>${c.maLop}</td>
                <td>Lớp ${c.maLop} - ${c.tenKhoaHoc || 'Khoá học chung'}</td>
                <td>${c.lichHoc || 'N/A'}</td>
                <td>${c.phongTap || 'N/A'}</td>
                <td>${c.soHocVienHienTai || 0}/${c.soLuongToiDa || 0}</td>
                <td>
                    <button class="btn btn-sm btn-outline-primary me-1" onclick="editClass(${c.maLop})"><i class="fas fa-edit"></i></button>
                    <button class="btn btn-sm btn-outline-danger" onclick="unregisterClass(${c.maLop})"><i class="fas fa-user-minus"></i></button>
                </td>
            </tr>`;
        });
    }
    document.getElementById("classListBody").innerHTML = html;
}

// Modal handling
let currentModalCallback = null;
function openModal(title, bodyHtml, onSave) {
    document.getElementById("modalTitle").innerHTML = title;
    document.getElementById("modalBody").innerHTML = bodyHtml;
    document.getElementById("genericModal").classList.add("show");
    currentModalCallback = onSave;
}
function closeModal() {
    document.getElementById("genericModal").classList.remove("show");
    currentModalCallback = null;
}
document.getElementById("modalSaveBtn").addEventListener('click', () => {
    if (currentModalCallback) currentModalCallback();
});

// Edit class (only limited fields like schedule, room)
function openClassModal(classData = null) {
    if (!classData) return; // Cannot add new class

    let html = `
        <div class="mb-2">
            <label class="small text-secondary">Lịch học</label>
            <input id="classSchedule" class="form-control" value="${classData.lichHoc || ''}">
        </div>
        <div class="mb-2">
            <label class="small text-secondary">Phòng tập</label>
            <select id="classRoom" class="form-select">
                <option value="Phòng A" ${classData.phongTap === 'Phòng A' ? 'selected' : ''}>Phòng A</option>
                <option value="Phòng B" ${classData.phongTap === 'Phòng B' ? 'selected' : ''}>Phòng B</option>
                <option value="Phòng C" ${classData.phongTap === 'Phòng C' ? 'selected' : ''}>Phòng C</option>
                <option value="Sân ngoài trời" ${classData.phongTap === 'Sân ngoài trời' ? 'selected' : ''}>Sân ngoài trời</option>
            </select>
        </div>
    `;

    openModal("Sửa thông tin lớp học", html, async () => {
        let schedule = document.getElementById("classSchedule").value;
        let room = document.getElementById("classRoom").value;

        if (!schedule) return toast("Vui lòng điền lịch học", "warning");

        const payload = {
            lichHoc: schedule,
            phongTap: room
        };

        try {
            // Use admin endpoint to update class since trainer may not have permission
            let res = await apiCall(`/admin/lophoc/${classData.maLop}`, 'PUT', payload);
            if (res.success) {
                toast("Cập nhật lớp thành công", "success");
                await fetchClasses();
                renderClassList();
                renderClassSelects();
                closeModal();
            } else {
                console.error("Lỗi cập nhật lớp:", res);
                toast(res.data?.message || "Có lỗi xảy ra", "error");
            }
        } catch (e) {
            console.error("Lỗi cập nhật lớp:", e);
            toast("Lỗi hệ thống", "error");
        }
    });
}

function editClass(id) {
    let c = classes.find(c => c.maLop === id);
    if (c) openClassModal(c);
}

async function unregisterClass(id) {
    let html = `
        <div class="mb-2">
            <label class="small text-secondary">Lý do hủy đăng ký</label>
            <textarea id="unregisterReason" class="form-control" rows="3" placeholder="Nhập lý do hủy đăng ký lớp này..."></textarea>
        </div>
    `;

    openModal("Hủy đăng ký lớp dạy", html, async () => {
        let reason = document.getElementById("unregisterReason").value;

        if (!reason) return toast("Vui lòng nhập lý do", "warning");

        try {
            // Use admin endpoint to unregister since trainer may not have permission
            const res = await apiCall(`/admin/lophoc/${id}/unregister`, 'POST', { reason: reason });
            if (res.success) {
                toast("Đã hủy đăng ký lớp", "success");
                await fetchClasses();
                renderClassList();
                renderClassSelects();
                closeModal();
            } else {
                console.error("Lỗi hủy đăng ký:", res);
                toast(res.data?.message || "Lỗi khi hủy đăng ký", "error");
            }
        } catch (e) {
            console.error("Lỗi hủy đăng ký:", e);
            toast("Lỗi hệ thống", "error");
        }
    });
}

// -----------------------------------------
// STUDENT CRUD (Add existing students to assigned classes)
// -----------------------------------------
async function fetchStudents() {
    try {
        const result = await apiCall('/trainer/students');
        if (result.success && result.data && result.data.success) {
            students = result.data.data;
            saveToStorage(STORAGE_KEYS.STUDENTS, students);
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách học viên", e);
        // Load from localStorage if API fails
        students = loadFromStorage(STORAGE_KEYS.STUDENTS, []);
    }
}

// Fetch all students from system (not just trainer's classes)
async function fetchAllStudents() {
    try {
        const result = await apiCall('/admin/hocvien');
        if (result.success && result.data && result.data.success) {
            return result.data.data;
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách học viên toàn hệ thống", e);
    }
    return [];
}

function renderStudentTable() {
    let search = document.getElementById("searchStudent")?.value.toLowerCase() || "";
    let filterClass = document.getElementById("filterClass")?.value || "";

    let filtered = students.filter(s => {
        let matchName = (s.tenHocVien || "").toLowerCase().includes(search) || (s.email || "").toLowerCase().includes(search);
        let matchClass = (filterClass === "") || (s.maLop == filterClass);
        return matchName && matchClass;
    });

    let html = "";
    if (filtered.length === 0) {
        html = `<tr><td colspan="6" class="text-center">Không có học viên nào</td></tr>`;
    } else {
        filtered.forEach(s => {
            html += `<tr>
                <td>HV${s.maHocVien}</td>
                <td>${s.tenHocVien}</td>
                <td>${s.email}</td>
                <td>${s.tenCapDai || 'Chưa có'}</td>
                <td><span class="class-badge">${s.tenLop || 'Lớp '+s.maLop}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-info me-1" onclick="viewStudent(${s.maHocVien})"><i class="fas fa-eye"></i></button>
                    <button class="btn btn-sm btn-outline-danger" onclick="removeStudentFromClass(${s.maHocVien})"><i class="fas fa-user-minus"></i></button>
                </td>
            </tr>`;
        });
    }
    document.getElementById("studentTableBody").innerHTML = html;
}

function viewStudent(id) {
    let s = students.find(s => s.maHocVien === id);
    if (s) {
        let html = `
            <p><strong>Họ Tên:</strong> ${s.tenHocVien}</p>
            <p><strong>Email:</strong> ${s.email}</p>
            <p><strong>SĐT:</strong> ${s.soDienThoai || 'N/A'}</p>
            <p><strong>Cấp đai:</strong> ${s.tenCapDai || 'N/A'}</p>
            <p><strong>Lớp:</strong> ${s.tenLop}</p>
        `;
        openModal("Thông tin học viên", html, () => { closeModal(); });
    }
}

// Open modal to add existing student to class
function openStudentModal() {
    let classOptions = classes.map(c => `<option value="${c.maLop}">Lớp ${c.maLop} - ${c.tenKhoaHoc || 'Khoá học'}</option>`).join("");

    let html = `
        <div class="mb-2">
            <label class="small text-secondary">Chọn lớp</label>
            <select id="studentClass" class="form-select" onchange="loadAvailableStudents()">
                <option value="">Chọn lớp trước</option>
                ${classOptions}
            </select>
        </div>
        <div class="mb-2" id="availableStudentsDiv" style="display:none;">
            <label class="small text-secondary">Chọn học viên có sẵn</label>
            <select id="studentSelect" class="form-select">
                <option value="">Chọn học viên</option>
            </select>
        </div>
    `;

    openModal("Thêm học viên vào lớp", html, async () => {
        let maLop = parseInt(document.getElementById("studentClass").value);
        let maHocVien = parseInt(document.getElementById("studentSelect").value);

        if (!maLop || !maHocVien) {
            return toast("Vui lòng chọn lớp và học viên", "warning");
        }

        try {
            // Use admin endpoint to create registration (DangKyLop)
            const res = await apiCall('/admin/dangkylop', 'POST', {
                maHocVien: maHocVien,
                maLop: maLop,
                ngayDangKy: new Date().toISOString().split('T')[0],
                trangThaiThanhToan: "ChuaThanhToan"
            });

            if (res.success) {
                toast("Thêm học viên vào lớp thành công", "success");
                // Re-fetch students to update the list
                await fetchStudents();
                await fetchAllStudents(); // Also refresh all students for the dropdown
                renderStudentTable();
                closeModal();
                console.log("Student added successfully, refreshed data");
            } else {
                console.error("Lỗi thêm học viên:", res);
                toast(res.data?.message || "Có lỗi xảy ra", "error");
            }
        } catch (e) {
            console.error("Lỗi thêm học viên:", e);
            toast("Lỗi hệ thống", "error");
        }
    });
}

// Load available students (not in any class or in other classes)
async function loadAvailableStudents() {
    let maLop = document.getElementById("studentClass").value;
    if (!maLop) {
        document.getElementById("availableStudentsDiv").style.display = 'none';
        return;
    }

    document.getElementById("availableStudentsDiv").style.display = 'block';

    try {
        let allStudents = await fetchAllStudents();
        let classStudentIds = students.filter(s => s.maLop == maLop).map(s => s.maHocVien);

        // Filter students not in this class
        let availableStudents = allStudents.filter(s => !classStudentIds.includes(s.maHocVien));

        let options = availableStudents.map(s => `<option value="${s.maHocVien}">${s.tenHocVien} (HV${s.maHocVien}) - ${s.tenCapDai || 'Chưa có cấp đai'}</option>`).join("");
        document.getElementById("studentSelect").innerHTML = `<option value="">Chọn học viên</option>${options}`;
    } catch (e) {
        console.error("Lỗi tải danh sách học viên:", e);
        toast("Lỗi tải danh sách học viên", "error");
    }
}

async function removeStudentFromClass(id) {
    if (confirm("Xóa học viên này khỏi lớp?")) {
        try {
            // Get the student's current class from the students array
            let student = students.find(s => s.maHocVien === id);
            if (!student) {
                toast("Không tìm thấy học viên", "error");
                return;
            }

            let classId = student.maLop;
            if (!classId) {
                toast("Học viên này chưa có lớp", "error");
                return;
            }

            // Fetch registrations for the student's class to find the registration ID
            const res = await apiCall(`/admin/dangkylop/by-lop/${classId}`);
            if (!res.success || !res.data || !res.data.success) {
                toast("Lỗi lấy thông tin đăng ký", "error");
                return;
            }

            let registrations = res.data.data;
            let reg = registrations.find(r => r.maHocVien === id);
            
            if (!reg) {
                toast("Không tìm thấy thông tin đăng ký", "error");
                return;
            }

            // Use admin endpoint to delete the registration
            const deleteRes = await apiCall(`/admin/dangkylop/${reg.maDangKy}`, 'DELETE');
            if (deleteRes.success) {
                toast("Đã xoá học viên khỏi lớp", "success");
                await fetchStudents();
                renderStudentTable();
            } else {
                console.error("Lỗi xoá học viên:", deleteRes);
                toast(deleteRes.data?.message || "Lỗi khi xoá", "error");
            }
        } catch (e) {
            console.error("Lỗi xoá học viên:", e);
            toast("Lỗi hệ thống", "error");
        }
    }
}
// -----------------------------------------
// ATTENDANCE (Dynamic data from API)
// -----------------------------------------
function renderClassSelects() {
    let opts = "<option value=''>Chọn lớp</option>";
    classes.forEach(c => opts += `<option value="${c.maLop}">Lớp ${c.maLop} - ${c.tenKhoaHoc || 'Khoá học'}</option>`);

    const attSelect = document.getElementById("attClassSelect");
    if (attSelect) attSelect.innerHTML = opts;

    const filterSelect = document.getElementById("filterClass");
    if (filterSelect) {
        filterSelect.innerHTML = "<option value=''>Tất cả lớp</option>" + classes.map(c => `<option value="${c.maLop}">Lớp ${c.maLop}</option>`).join("");
    }

    const examSelect = document.getElementById("examSelect");
    if (examSelect) {
        examSelect.innerHTML = "<option value=''>Chọn kỳ thi</option>" + exams.map(e => `<option value="${e.maKyThi}">${e.tenKhoaHoc || 'Kỳ thi'} (${e.ngayThi})</option>`).join("");
    }
}

async function loadAttendanceSheet() {
    let classId = document.getElementById("attClassSelect").value;
    let date = document.getElementById("attDate").value;

    if (!classId) {
        document.getElementById("attendanceSheet").innerHTML = "";
        return;
    }

    if (!date) {
        document.getElementById("attendanceSheet").innerHTML = `<p class="text-muted small">Vui lòng chọn ngày để xem bảng điểm danh.</p>`;
        return;
    }

    // Fetch attendance for this date & class from API
    let existingRecs = [];
    try {
        const res = await apiCall(`/trainer/attendance?maLop=${classId}&ngayHoc=${date}`);
        if (res.success && res.data && res.data.success) {
            existingRecs = res.data.data;
        }
    } catch(e) {
        console.error("Lỗi lấy điểm danh cũ:", e);
    }

    // Fetch registrations for this class to get MaDangKy
    let registrations = [];
    try {
        const res = await apiCall(`/admin/dangkylop/by-lop/${classId}`);
        if (res.success && res.data && res.data.success) {
            registrations = res.data.data;
        }
    } catch(e) {
        console.error("Lỗi lấy đăng ký lớp:", e);
    }

    // Get students for this class from API (dynamic data)
    let classStudents = students.filter(s => s.maLop == classId);

    console.log("Class ID:", classId, "Date:", date);
    console.log("All students:", students);
    console.log("Class students:", classStudents);
    console.log("Registrations:", registrations);
    console.log("Existing attendance records:", existingRecs);

    if (classStudents.length === 0) {
        document.getElementById("attendanceSheet").innerHTML = `<p class="text-warning small"><i class="fas fa-exclamation-triangle"></i> Lớp này chưa có học viên.</p>`;
        return;
    }

    // Build attendance sheet with dynamic data
    let html = `<table class="table table-bordered table-sm">
        <thead>
            <tr>
                <th>Mã HV</th>
                <th>Họ tên</th>
                <th>Trạng thái</th>
            </tr>
        </thead>
        <tbody>`;

    classStudents.forEach(s => {
        let existingRec = existingRecs.find(r => r.maHocVien === s.maHocVien);
        let trangThai = existingRec ? existingRec.trangThai : 'CoMat';
        
        // Find registration for this student
        let reg = registrations.find(r => r.maHocVien === s.maHocVien);
        let maDangKy = reg ? reg.maDangKy : '';

        html += `<tr>
            <td>HV${s.maHocVien}</td>
            <td>${s.tenHocVien}</td>
            <td>
                <select class="form-select form-select-sm att-status" data-ma-hv="${s.maHocVien}" data-ma-dk="${maDangKy}">
                    <option value="CoMat" ${trangThai === 'CoMat' ? 'selected' : ''}>Có mặt</option>
                    <option value="VangPhep" ${trangThai === 'VangPhep' ? 'selected' : ''}>Vắng có phép</option>
                    <option value="VangKhongPhep" ${trangThai === 'VangKhongPhep' ? 'selected' : ''}>Vắng không phép</option>
                </select>
            </td>
        </tr>`;
    });

    html += `</tbody></table>`;
    document.getElementById("attendanceSheet").innerHTML = html;
}

async function saveAttendance() {
    let classId = document.getElementById("attClassSelect").value;
    let date = document.getElementById("attDate").value;

    if (!classId || !date) {
        toast("Vui lòng chọn lớp và ngày", "warning");
        return;
    }

    let classStudents = students.filter(s => s.maLop == classId);
    let attendanceData = [];

    document.querySelectorAll('.att-status').forEach(select => {
        let maDangKy = parseInt(select.getAttribute('data-ma-dk'));
        let trangThai = select.value;
        if (maDangKy) {
            attendanceData.push({
                maDangKy: maDangKy,
                trangThai: trangThai,
                ghiChu: ""
            });
        }
    });

    console.log("Attendance data to save:", attendanceData);
    console.log("Number of .att-status elements:", document.querySelectorAll('.att-status').length);
    console.log("Class ID:", classId, "Date:", date);

    if (attendanceData.length === 0) {
        toast("Không có học viên để điểm danh", "warning");
        return;
    }

    try {
        // Use admin single attendance endpoint to avoid bulk SQL constraint issue
        let successCount = 0;
        let errorCount = 0;

        for (let item of attendanceData) {
            try {
                const res = await apiCall(`/admin/dangkylop/${item.maDangKy}/diemdanh`, 'POST', {
                    ngayHoc: date,
                    trangThai: item.trangThai,
                    ghiChu: item.ghiChu
                });
                if (res.success) {
                    successCount++;
                } else {
                    errorCount++;
                    console.error("Lỗi điểm danh cho học viên:", item, res);
                }
            } catch (e) {
                errorCount++;
                console.error("Lỗi điểm danh cho học viên:", item, e);
            }
        }

        if (successCount > 0) {
            toast(`Đã lưu điểm danh thành công cho ${successCount} học viên${errorCount > 0 ? `, ${errorCount} lỗi` : ''}`, "success");
            await loadAttendanceSheet();
        } else {
            toast("Lỗi khi lưu điểm danh", "error");
        }
    } catch (e) {
        console.error("Lỗi điểm danh:", e);
        toast("Lỗi hệ thống", "error");
    }
}

// -----------------------------------------
// EXAMS (View only, view results by course)
// -----------------------------------------
async function fetchExams() {
    try {
        const result = await apiCall('/trainer/exams');
        if (result.success && result.data.success) {
            exams = result.data.data;
            saveToStorage(STORAGE_KEYS.EXAMS, exams);
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách kỳ thi", e);
        // Load from localStorage if API fails
        exams = loadFromStorage(STORAGE_KEYS.EXAMS, []);
    }
}

function renderExamList() {
    let html = "";
    if (exams.length === 0) {
        html = `<tr><td colspan="4" class="text-center">Chưa có kỳ thi nào</td></tr>`;
    } else {
        exams.forEach(ex => {
            html += `<tr>
                <td>${ex.tenKhoaHoc || 'Kỳ thi'}</td>
                <td>${ex.ngayThi}</td>
                <td>Khoá học: ${ex.maKhoaHoc}</td>
                <td>
                    <button class="btn btn-sm btn-outline-info" onclick="viewExamResults(${ex.maKyThi})">Xem kết quả</button>
                </td>
            </tr>`;
        });
    }
    document.getElementById("examListBody").innerHTML = html;
}

// View exam results filtered by trainer's classes
async function viewExamResults(examId) {
    let ex = exams.find(e => e.maKyThi === examId);
    if (!ex) return;

    // Fetch exam results
    try {
        // Use admin endpoint for exam results
        const res = await apiCall(`/admin/kythithangdai/${examId}/ketqua`);
        if (res.success && res.data && res.data.success) {
            let results = res.data.data;

            // Filter results by trainer's students (not by class since results don't have class info)
            let trainerStudentIds = students.map(s => s.maHocVien);
            let filteredResults = results.filter(r => trainerStudentIds.includes(r.maHocVien));

            let html = `
                <h6>Kết quả kỳ thi: ${ex.tenKhoaHoc || 'Kỳ thi'}</h6>
                <p class="small text-secondary">Ngày thi: ${ex.ngayThi}</p>
                <hr>
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>Mã HV</th>
                            <th>Họ tên</th>
                            <th>Điểm</th>
                            <th>Kết quả</th>
                        </tr>
                    </thead>
                    <tbody>
            `;

            if (filteredResults.length === 0) {
                html += `<tr><td colspan="4" class="text-center">Không có học viên của bạn tham gia kỳ thi này hoặc admin chưa nhập kết quả</td></tr>`;
            } else {
                filteredResults.forEach(r => {
                    html += `<tr>
                        <td>HV${r.maHocVien}</td>
                        <td>${r.tenHocVien}</td>
                        <td>${r.diemSo || 'N/A'}</td>
                        <td><span class="badge ${r.daDat ? 'bg-success' : 'bg-danger'}">${r.daDat ? 'Đạt' : 'Không đạt'}</span></td>
                    </tr>`;
                });
            }

            html += `</tbody></table>`;

            openModal("Kết quả thi", html, () => { closeModal(); });
        } else {
            console.error("Lỗi lấy kết quả thi:", res);
            toast(res.data?.message || "Lỗi lấy kết quả thi", "error");
        }
    } catch (e) {
        console.error("Lỗi lấy kết quả thi:", e);
        toast("Lỗi hệ thống", "error");
    }
}

// -----------------------------------------
// EVENTS (Manage only trainer's classes/students)
// -----------------------------------------
async function fetchEvents() {
    try {
        // Try trainer endpoint first
        const result = await apiCall('/trainer/events');
        if (result.success && result.data && result.data.success) {
            events = result.data.data;
            saveToStorage(STORAGE_KEYS.EVENTS, events);
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách sự kiện:", e);
        // Load from localStorage if API fails
        events = loadFromStorage(STORAGE_KEYS.EVENTS, []);
    }
}

function renderEventList() {
    let html = "";
    if (events.length === 0) {
        html = `<p class="text-center text-muted">Chưa có sự kiện nào</p>`;
    } else {
        events.forEach(ev => {
            html += `<div class="border-bottom p-2 mb-2">
                <h6>${ev.tieuDe}</h6>
                <small class="text-secondary">${ev.ngayDang} - ${ev.loaiThongBao}</small>
                <p class="small mt-1">${ev.noiDung}</p>
                <div class="mt-2">
                    <button class="btn btn-sm btn-outline-primary" onclick="editEvent(${ev.maThongBao})">Sửa</button>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteEvent(${ev.maThongBao})">Xóa</button>
                </div>
            </div>`;
        });
    }
    document.getElementById("eventList").innerHTML = html;
}

function openEventModal(eventData = null) {
    let isEdit = !!eventData;

    // Get trainer's classes and students
    let trainerClassIds = classes.map(c => c.maLop);
    let trainerStudentIds = students.map(s => s.maHocVien);

    let classOptions = classes.map(c => `<option value="${c.maLop}">Lớp ${c.maLop} - ${c.tenKhoaHoc || 'Khoá học'}</option>`).join("");
    let studentOptions = students.map(s => `<option value="${s.maHocVien}">${s.tenHocVien} (HV${s.maHocVien})</option>`).join("");

    let html = `
        <div class="mb-2">
            <label class="small text-secondary">Tiêu đề</label>
            <input id="eventTitle" class="form-control" value="${eventData ? eventData.tieuDe : ''}">
        </div>
        <div class="mb-2">
            <label class="small text-secondary">Nội dung</label>
            <textarea id="eventDesc" class="form-control" rows="4">${eventData ? eventData.noiDung : ''}</textarea>
        </div>
        <div class="mb-2">
            <label class="small text-secondary">Loại thông báo</label>
            <select id="eventType" class="form-select">
                <option value="SuKien" ${eventData && eventData.loaiThongBao === 'SuKien' ? 'selected' : ''}>Sự kiện</option>
                <option value="ThongBao" ${eventData && eventData.loaiThongBao === 'ThongBao' ? 'selected' : ''}>Thông báo</option>
            </select>
        </div>
        <div class="mb-2">
            <label class="small text-secondary">Đối tượng gửi</label>
            <select id="eventTarget" class="form-select" onchange="toggleEventTargetSelect()">
                <option value="All" ${eventData && !eventData.maLop && !eventData.maHocVien ? 'selected' : ''}>Tất cả</option>
                <option value="Class" ${eventData && eventData.maLop ? 'selected' : ''}>Theo lớp</option>
                <option value="Student" ${eventData && eventData.maHocVien ? 'selected' : ''}>Theo học viên</option>
            </select>
        </div>
        <div class="mb-2" id="classSelectDiv" style="display:${eventData && eventData.maLop ? 'block' : 'none'};">
            <label class="small text-secondary">Chọn lớp (của bạn)</label>
            <select id="eventClass" class="form-select">
                <option value="">Chọn lớp</option>
                ${classOptions}
            </select>
        </div>
        <div class="mb-2" id="studentSelectDiv" style="display:${eventData && eventData.maHocVien ? 'block' : 'none'};">
            <label class="small text-secondary">Chọn học viên (của bạn)</label>
            <select id="eventStudent" class="form-select">
                <option value="">Chọn học viên</option>
                ${studentOptions}
            </select>
        </div>
    `;

    // After setting HTML, set the selected values for edit mode
    setTimeout(() => {
        if (eventData && eventData.maLop) {
            let classSelect = document.getElementById("eventClass");
            if (classSelect) classSelect.value = eventData.maLop;
        }
        if (eventData && eventData.maHocVien) {
            let studentSelect = document.getElementById("eventStudent");
            if (studentSelect) studentSelect.value = eventData.maHocVien;
        }
    }, 0);

    openModal(isEdit ? "Sửa sự kiện" : "Tạo sự kiện", html, async () => {
        let payload = {
            tieuDe: document.getElementById("eventTitle").value,
            noiDung: document.getElementById("eventDesc").value,
            loaiThongBao: document.getElementById("eventType").value
        };

        let target = document.getElementById("eventTarget").value;
        if (target === 'Class') {
            let maLop = parseInt(document.getElementById("eventClass").value);
            if (!maLop) return toast("Vui lòng chọn lớp", "warning");
            payload.maLop = maLop;
        } else if (target === 'Student') {
            let maHocVien = parseInt(document.getElementById("eventStudent").value);
            if (!maHocVien) return toast("Vui lòng chọn học viên", "warning");
            payload.maHocVien = maHocVien;
        }

        try {
            let res;
            if (isEdit) {
                // Use admin endpoint for edit
                res = await apiCall(`/admin/thongbao/${eventData.maThongBao}`, 'PUT', payload);
            } else {
                // Use admin endpoint for create with trainer's account ID (hardcoded for now)
                payload.maTaiKhoanTao = 1; // Trainer's account ID
                res = await apiCall('/admin/thongbao', 'POST', payload);
            }

            if (res.success) {
                toast(isEdit ? "Cập nhật sự kiện thành công" : "Tạo sự kiện thành công", "success");
                await fetchEvents();
                renderEventList();
                closeModal();
            } else {
                console.error("Lỗi sự kiện:", res);
                toast(res.data?.message || "Có lỗi xảy ra", "error");
            }
        } catch(e) {
            console.error("Lỗi sự kiện:", e);
            toast("Lỗi hệ thống", "error");
        }
    });
}

function editEvent(id) {
    let ev = events.find(e => e.maThongBao === id);
    if (ev) openEventModal(ev);
}

async function deleteEvent(id) {
    if (confirm("Xóa sự kiện này?")) {
        try {
            // Use admin endpoint for delete
            const res = await apiCall(`/admin/thongbao/${id}`, 'DELETE');
            if (res.success) {
                toast("Đã xoá sự kiện", "success");
                await fetchEvents();
                renderEventList();
            } else {
                console.error("Lỗi xoá sự kiện:", res);
                toast(res.data?.message || "Lỗi khi xoá", "error");
            }
        } catch(e) {
            console.error("Lỗi xoá sự kiện:", e);
            toast("Lỗi hệ thống", "error");
        }
    }
}

function toggleEventTargetSelect() {
    let target = document.getElementById("eventTarget").value;
    document.getElementById("classSelectDiv").style.display = target === 'Class' ? 'block' : 'none';
    document.getElementById("studentSelectDiv").style.display = target === 'Student' ? 'block' : 'none';
}

// -----------------------------------------
// PROFILE
// -----------------------------------------
async function fetchProfile() {
    try {
        const result = await apiCall('/trainer/profile');
        if (result.success && result.data.success) {
            currentTrainerProfile = result.data.data;
            document.getElementById("profileName").value = currentTrainerProfile.hoTen || '';
            document.getElementById("profileEmail").value = currentTrainerProfile.email || '';
            document.getElementById("profileChuyenMon").value = currentTrainerProfile.chuyenMon || '';
            saveToStorage(STORAGE_KEYS.PROFILE, currentTrainerProfile);
        }
    } catch (e) {
        console.error("Lỗi lấy hồ sơ", e);
        // Load from localStorage if API fails
        const cachedProfile = loadFromStorage(STORAGE_KEYS.PROFILE);
        if (cachedProfile) {
            currentTrainerProfile = cachedProfile;
            document.getElementById("profileName").value = currentTrainerProfile.hoTen || '';
            document.getElementById("profileEmail").value = currentTrainerProfile.email || '';
            document.getElementById("profileChuyenMon").value = currentTrainerProfile.chuyenMon || '';
        }
    }
}

async function saveProfile() {
    if (!currentTrainerProfile) return;

    let payload = {
        hoTen: document.getElementById("profileName").value,
        soDienThoai: currentTrainerProfile.soDienThoai,
        chuyenMon: document.getElementById("profileChuyenMon").value
    };

    try {
        const res = await apiCall('/trainer/profile', 'PUT', payload);
        if (res.success) {
            toast("Cập nhật hồ sơ thành công", "success");
            await fetchProfile(); // Re-fetch and save to localStorage
        } else {
            toast(res.data?.message || "Có lỗi xảy ra", "error");
        }
    } catch (e) {
        toast("Lỗi hệ thống", "error");
    }
}

// UI Setup functions
function showSection(id){
    document.querySelectorAll('.section').forEach(s => s.classList.remove('active'));
    const target = document.getElementById(id);
    if(target) target.classList.add('active');

    document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
    const activeLink = document.querySelector(`.nav-link[onclick="showSection('${id}')"]`);
    if(activeLink) {
        activeLink.classList.add('active');
        document.getElementById('section-title').innerHTML = activeLink.innerText.toUpperCase();
    }

    // Save current section to localStorage
    saveToStorage(STORAGE_KEYS.CURRENT_SECTION, id);

    if (id === 'dashboard') updateChart();
}

function toast(msg, type = 'success') {
    const colors = { success: '#1e8449', error: '#c0392b', warning: '#9a7d0a', info: '#2980b9' };
    const icons = { success: 'fa-check-circle', error: 'fa-times-circle', warning: 'fa-exclamation-triangle', info: 'fa-info-circle' };
    const el = document.createElement('div');
    el.style.cssText = `
      position:fixed;bottom:24px;right:24px;
      background:${colors[type] || colors.info};
      color:#fff;padding:12px 18px;border-radius:8px;
      font-size:.82rem;font-weight:600;font-family:Montserrat,sans-serif;
      display:flex;align-items:center;gap:9px;
      box-shadow:0 4px 20px rgba(0,0,0,.4);
      z-index:9999;animation:fadeUp .2s ease;
    `;
    el.innerHTML = `<i class="fas ${icons[type] || icons.info}"></i> ${msg}`;
    document.body.appendChild(el);
    setTimeout(() => {
      el.style.opacity = '0';
      el.style.transition = 'opacity .3s';
      setTimeout(() => el.remove(), 300);
    }, 3000);
}
