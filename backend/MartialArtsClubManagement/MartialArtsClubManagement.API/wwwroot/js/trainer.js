// js/trainer.js

// Global Data
let classes = [];
let students = [];
let attendanceRecords = [];
let exams = [];
let events = [];
let currentTrainerProfile = null;

// Initialization
document.addEventListener('DOMContentLoaded', async () => {
    // Check if user is logged in and is a trainer
    if (!checkAuth(['HuanLuyenVien', 'Trainer'])) return;

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
        }
    } catch (e) {
        console.error("Lỗi lấy dashboard", e);
    }
}

let chart;
function updateChart() {
    const ctx = document.getElementById("attChart");
    if (!ctx) return;
    
    // Use some dummy data for the chart, or real data if we have historical API
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
// CLASS CRUD
// -----------------------------------------
async function fetchClasses() {
    try {
        const result = await apiCall('/trainer/classes');
        if (result.success && result.data && result.data.success) {
            classes = result.data.data;
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách lớp", e);
    }
}

function renderClassList() {
    let html = "";
    classes.forEach(c => {
        html += `<tr>
            <td>${c.maLop}</td>
            <td>Lớp ${c.maLop} - ${c.tenKhoaHoc || 'Khoá học chung'}</td>
            <td>${c.lichHoc}</td>
            <td>${c.phongTap}</td>
            <td>${c.soHocVienHienTai}/${c.soLuongToiDa}</td>
            <td>
                <!-- Currently TrainerController doesn't support creating/editing classes (they belong to Admin). Wait, the API does have POST/PUT/DELETE /api/trainer/classes. -->
                <button class="btn btn-sm btn-outline-primary me-1" onclick="editClass(${c.maLop})"><i class="fas fa-edit"></i></button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteClass(${c.maLop})"><i class="fas fa-trash"></i></button>
            </td>
        </tr>`;
    });
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

// Assuming MaKhoaHoc and MaCapDai are required
function openClassModal(classData = null) {
    let isEdit = !!classData;
    let html = `
        <div class="mb-2">
            <label class="small text-secondary">Mã Khoá Học (ID)</label>
            <input id="classKhoaHoc" type="number" class="form-control" value="${classData ? classData.maKhoaHoc : ''}">
        </div>
        <div class="mb-2">
            <label class="small text-secondary">Mã Cấp Đai (ID)</label>
            <input id="classCapDai" type="number" class="form-control" value="${classData ? classData.maCapDai : ''}">
        </div>
        <input id="classSchedule" class="form-control mb-2" placeholder="Lịch học (VD: T2-T4-T6)" value="${classData ? classData.lichHoc : ''}">
        <input id="classRoom" class="form-control mb-2" placeholder="Phòng tập" value="${classData ? classData.phongTap : ''}">
        <input id="classMax" type="number" class="form-control mb-2" placeholder="Số lượng tối đa" value="${classData ? classData.soLuongToiDa : '30'}">
    `;
    
    openModal(isEdit ? "Sửa lớp học" : "Thêm lớp học", html, async () => {
        let maKhoaHoc = document.getElementById("classKhoaHoc").value;
        let maCapDai = document.getElementById("classCapDai").value;
        let schedule = document.getElementById("classSchedule").value;
        let room = document.getElementById("classRoom").value;
        let max = document.getElementById("classMax").value;
        
        if (!maKhoaHoc || !maCapDai || !schedule) return toast("Vui lòng điền đủ thông tin", "warning");
        
        const payload = {
            maKhoaHoc: parseInt(maKhoaHoc),
            maCapDai: parseInt(maCapDai),
            lichHoc: schedule,
            phongTap: room,
            soLuongToiDa: parseInt(max)
        };
        
        try {
            let res;
            if (isEdit) {
                res = await apiCall(`/trainer/classes/${classData.maLop}`, 'PUT', payload);
            } else {
                res = await apiCall(`/trainer/classes`, 'POST', payload);
            }
            if (res.success) {
                toast(isEdit ? "Cập nhật lớp thành công" : "Thêm lớp thành công", "success");
                await renderAll();
                closeModal();
            } else {
                toast(res.data?.message || "Có lỗi xảy ra", "error");
            }
        } catch (e) {
            toast("Lỗi hệ thống", "error");
        }
    });
}

function editClass(id) {
    let c = classes.find(c => c.maLop === id);
    if (c) openClassModal(c);
}

async function deleteClass(id) {
    if (confirm("Xóa lớp học này? Mọi học viên trong lớp sẽ bị ảnh hưởng.")) {
        try {
            const res = await apiCall(`/trainer/classes/${id}`, 'DELETE');
            if (res.success) {
                toast("Đã xoá lớp", "success");
                await renderAll();
            } else {
                toast(res.data?.message || "Lỗi khi xoá", "error");
            }
        } catch (e) {
            toast("Lỗi hệ thống", "error");
        }
    }
}

// -----------------------------------------
// STUDENT CRUD (Read-only for trainer usually, but endpoint provides list)
// -----------------------------------------
async function fetchStudents() {
    try {
        const result = await apiCall('/trainer/students');
        if (result.success && result.data && result.data.success) {
            students = result.data.data;
        }
    } catch (e) {
        console.error("Lỗi lấy danh sách học viên", e);
    }
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
    filtered.forEach(s => {
        html += `<tr>
            <td>HV${s.maHocVien}</td>
            <td>${s.tenHocVien}</td>
            <td>${s.email}</td>
            <td>${s.tenCapDai || 'Chưa có'}</td>
            <td><span class="class-badge">${s.tenLop || 'Lớp '+s.maLop}</span></td>
            <td>
                <!-- Trainers typically do not edit student profiles, so viewing details only -->
                <button class="btn btn-sm btn-outline-info" onclick="viewStudent(${s.maHocVien})"><i class="fas fa-eye"></i> Xem</button>
            </td>
        </tr>`;
    });
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
// -----------------------------------------
// ATTENDANCE
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
    if (!classId) {
        document.getElementById("attendanceSheet").innerHTML = "";
        return;
    }
    
    let date = document.getElementById("attDate").value;
    
    // Fetch attendance for this date & class
    let existingRecs = [];
    try {
        const res = await apiCall(`/trainer/attendance?maLop=${classId}&ngayHoc=${date}`);
        if (res.success && res.data.success) {
            existingRecs = res.data.data;
        }
    } catch(e) {}

    // We also need the students to build the sheet. We need their MaDangKy.
    // However, TrainerHocVienDto doesn't return MaDangKy. We can assume MaHocVien mapping.
    // Wait, the API for bulk attendance requires MaDangKy. 
    // We should modify the frontend to just use standard attendance or we need the backend to return MaDangKy in students API.
    // If not, we will just display a message since we might not have MaDangKy.
    
    let html = `<p class="text-warning small"><i class="fas fa-exclamation-triangle"></i> Tính năng điểm danh cần MaDangKy, đang trong quá trình cập nhật API.</p>`;
    document.getElementById("attendanceSheet").innerHTML = html;
}

// -----------------------------------------
// EXAMS
// -----------------------------------------
async function fetchExams() {
    try {
        const result = await apiCall('/trainer/exams');
        if (result.success && result.data.success) {
            exams = result.data.data;
        }
    } catch (e) {}
}

function renderExamList() {
    let html = "";
    exams.forEach(ex => {
        html += `<tr>
            <td>${ex.tenKhoaHoc || 'Kỳ thi'}</td>
            <td>${ex.ngayThi}</td>
            <td>Khoá học: ${ex.maKhoaHoc}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="openExamModal(${ex.maKyThi})">Sửa</button>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteExam(${ex.maKyThi})">Xóa</button>
            </td>
        </tr>`;
    });
    document.getElementById("examListBody").innerHTML = html;
}

function openExamModal(id = null) {
    let ex = id ? exams.find(e => e.maKyThi === id) : null;
    let isEdit = !!ex;
    
    let courseOptions = [...new Set(classes.map(c => c.maKhoaHoc))].map(k => `<option value="${k}" ${ex && ex.maKhoaHoc == k ? "selected":""}>Khoá học ${k}</option>`).join("");
    
    let html = `
        <input id="examDate" type="date" class="form-control mb-2" value="${ex ? ex.ngayThi : ''}">
        <select id="examCourse" class="form-select mb-2">
            ${courseOptions}
        </select>
        <textarea id="examDesc" class="form-control mb-2" placeholder="Mô tả">${ex ? ex.moTa || '' : ''}</textarea>
    `;
    
    openModal(isEdit ? "Sửa kỳ thi" : "Thêm kỳ thi", html, async () => {
        let payload = {
            ngayThi: document.getElementById("examDate").value,
            maKhoaHoc: parseInt(document.getElementById("examCourse").value),
            moTa: document.getElementById("examDesc").value,
            trangThai: isEdit ? ex.trangThai : "SapDienRa"
        };
        
        try {
            let res = isEdit ? await apiCall(`/trainer/exams/${id}`, 'PUT', payload) : await apiCall(`/trainer/exams`, 'POST', payload);
            if (res.success) {
                toast("Lưu kỳ thi thành công", "success");
                await renderAll();
                closeModal();
            } else {
                toast(res.data?.message || "Có lỗi xảy ra", "error");
            }
        } catch(e) {
            toast("Lỗi hệ thống", "error");
        }
    });
}

async function deleteExam(id) {
    if (confirm("Xóa kỳ thi này?")) {
        try {
            const res = await apiCall(`/trainer/exams/${id}`, 'DELETE');
            if (res.success) {
                toast("Đã xoá kỳ thi", "success");
                await renderAll();
            }
        } catch(e){}
    }
}

// -----------------------------------------
// EVENTS
// -----------------------------------------
async function fetchEvents() {
    try {
        const result = await apiCall('/trainer/events');
        if (result.success && result.data.success) {
            events = result.data.data;
        }
    } catch (e) {}
}

function renderEventList() {
    let html = "";
    events.forEach(ev => {
        html += `<div class="border-bottom p-2 mb-2">
            <h6>${ev.tieuDe}</h6>
            <small class="text-secondary">${ev.ngayDang} - ${ev.loaiThongBao}</small>
            <p class="small mt-1">${ev.noiDung}</p>
        </div>`;
    });
    document.getElementById("eventList").innerHTML = html || "Chưa có sự kiện";
}

function openEventModal() {
    let html = `
        <input id="eventTitle" class="form-control mb-2" placeholder="Tiêu đề">
        <textarea id="eventDesc" class="form-control mb-2" placeholder="Nội dung" rows="4"></textarea>
        <select id="eventType" class="form-select mb-2">
            <option value="SuKien">Sự kiện</option>
            <option value="ThongBao">Thông báo</option>
        </select>
    `;
    
    openModal("Tạo thông báo / sự kiện", html, async () => {
        let payload = {
            tieuDe: document.getElementById("eventTitle").value,
            noiDung: document.getElementById("eventDesc").value,
            loaiThongBao: document.getElementById("eventType").value
        };
        
        try {
            let res = await apiCall(`/trainer/events`, 'POST', payload);
            if (res.success) {
                toast("Tạo sự kiện thành công", "success");
                await renderAll();
                closeModal();
            }
        } catch(e) {}
    });
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
        }
    } catch (e) {}
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
            await fetchProfile();
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
