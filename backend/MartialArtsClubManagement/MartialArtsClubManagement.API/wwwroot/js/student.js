// js/student.js

document.addEventListener('DOMContentLoaded', async () => {
    // Check if user is logged in and is a student
    if (!checkAuth(['HocVien', 'Student', 'User'])) return;

    // Determine which page we are on based on URL or DOM elements
    const path = window.location.pathname.toLowerCase();

    // Setup logout if exists (assuming there's a logout button somewhere in the menu, wait, the student UI close-btn goes to index.html, we should intercept or just let it be, but ideally there should be a logout button)
    
    // We can fetch profile data first as it's used in many places
    let profileData = null;
    try {
        const res = await apiCall('/HocVienPortal/profile');
        if (res.success && res.data.success) {
            profileData = res.data.data;
            // Update common UI elements if they exist
            const nameEl = document.getElementById('student-name');
            if (nameEl) nameEl.innerText = profileData.tenHocVien;
        }
    } catch(e) {}

    if (path.includes('dashboard.html')) {
        await initDashboard(profileData);
    } else if (path.includes('hoso.html')) {
        await initProfile(profileData);
    } else if (path.includes('lichtap.html')) {
        await initSchedule();
    } else if (path.includes('diemdanh.html')) {
        await initAttendance();
    } else if (path.includes('hocphi.html')) {
        await initTuition();
    } else if (path.includes('thanhtoan.html')) {
        await initPayment();
    } else if (path.includes('thangdai.html')) {
        await initExams();
    } else if (path.includes('thongbao.html')) {
        await initNotifications();
    } else if (path.includes('dangkylop.html')) {
        await initAvailableClasses();
    } else if (path.includes('dangkysukien.html')) {
        await initAvailableEvents();
    } else if (path.includes('caidat.html')) {
        await initSettings();
    }
});

async function initDashboard(profileData) {
    if (profileData) {
        const beltEl = document.getElementById('dashboard-belt');
        if (beltEl) beltEl.innerText = profileData.capDaiHienTai;
    }

    try {
        // Attendance count
        const attRes = await apiCall('/HocVienPortal/attendance');
        if (attRes.success && attRes.data.success) {
            const currentMonth = new Date().getMonth();
            const count = attRes.data.data.filter(d => d.trangThai === 'CóMặt' && new Date(d.ngayHoc).getMonth() === currentMonth).length;
            const cntEl = document.getElementById('attendance-count');
            if (cntEl) cntEl.innerText = count;
        }

        // Exam count
        const exRes = await apiCall('/HocVienPortal/exams');
        if (exRes.success && exRes.data.success) {
            const exCntEl = document.getElementById('exam-count');
            if (exCntEl) exCntEl.innerText = exRes.data.data.length;
        }

        // Schedule table
        const schRes = await apiCall('/HocVienPortal/schedule');
        if (schRes.success && schRes.data.success) {
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                schRes.data.data.forEach(item => {
                    html += `<tr>
                        <td>${item.loai}</td>
                        <td>${item.lich}</td>
                        <td>${item.ten}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="3" class="text-center">Chưa có lịch tập</td></tr>';
            }
        }
    } catch (e) { console.error(e); }
}

async function initProfile(profileData) {
    if (!profileData) return;

    // Update display fields
    const mappings = {
        'hoTen': profileData.tenHocVien,
        'maHocVien': 'HV' + profileData.maHocVien,
        'email': profileData.email,
        'sdt': profileData.soDienThoai,
        'diaChi': profileData.diaChi,
        'ngaySinh': profileData.ngaySinh,
        'gioiTinh': profileData.gioiTinh,
        'capDai': profileData.capDaiHienTai,
        'ngayGiaNhap': profileData.ngayGiaNhap
    };

    for (let key in mappings) {
        let el = document.getElementById(key);
        if (el) {
            if (el.tagName === 'INPUT' || el.tagName === 'SELECT') el.value = mappings[key] || '';
            else el.innerText = mappings[key] || '';
        }
    }

    // Update input fields
    const sdtInput = document.getElementById('sdtInput');
    if (sdtInput) sdtInput.value = profileData.soDienThoai || '';

    const diaChiInput = document.getElementById('diaChiInput');
    if (diaChiInput) diaChiInput.value = profileData.diaChi || '';

    const saveBtn = document.getElementById('btnSaveProfile');
    if (saveBtn) {
        saveBtn.addEventListener('click', async () => {
            const sdt = document.getElementById('sdtInput')?.value;
            const diaChi = document.getElementById('diaChiInput')?.value;
            try {
                const res = await apiCall('/HocVienPortal/profile', 'PUT', { SoDienThoai: sdt, DiaChi: diaChi });
                if (res.success && res.data.success) {
                    toast('Cập nhật hồ sơ thành công', 'success');
                    // Refresh profile data
                    const profileRes = await apiCall('/HocVienPortal/profile');
                    if (profileRes.success && profileRes.data.success) {
                        await initProfile(profileRes.data.data);
                    }
                } else {
                    toast(res.data?.message || 'Cập nhật thất bại', 'error');
                }
            } catch(e) {
                toast('Lỗi hệ thống', 'error');
            }
        });
    }
}

async function initSchedule() {
    try {
        const schRes = await apiCall('/HocVienPortal/schedule');
        if (schRes.success && schRes.data.success) {
            const tbody = document.getElementById('scheduleTableBody');
            if (tbody) {
                let html = '';
                schRes.data.data.forEach(item => {
                    html += `<tr>
                        <td>${item.loai}</td>
                        <td>${item.ten}</td>
                        <td>${item.lich}</td>
                        <td>${item.ngayDangKy}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="4" class="text-center">Chưa đăng ký lớp nào</td></tr>';
            }
        }
    } catch(e) {}
}

async function initAttendance() {
    try {
        const attRes = await apiCall('/HocVienPortal/attendance');
        if (attRes.success && attRes.data.success) {
            const container = document.getElementById('attendanceContainer');
            if (container) {
                let presentCount = 0;
                let lateCount = 0;
                let absentCount = 0;

                let html = '<div class="table-card"><h2>Lịch sử điểm danh</h2><table class="table table-dark"><thead><tr><th>Ngày</th><th>Lớp</th><th>Trạng thái</th></tr></thead><tbody>';

                attRes.data.data.forEach(item => {
                    let statusClass = '';
                    let statusText = '';

                    if (item.trangThai === 'CoMat' || item.trangThai === 'CóMặt') {
                        presentCount++;
                        statusClass = 'text-success';
                        statusText = 'Có mặt';
                    } else if (item.trangThai === 'VangPhep' || item.trangThai === 'Vắng có phép') {
                        absentCount++;
                        statusClass = 'text-warning';
                        statusText = 'Vắng có phép';
                    } else if (item.trangThai === 'VangKhongPhep' || item.trangThai === 'Vắng không phép') {
                        absentCount++;
                        statusClass = 'text-danger';
                        statusText = 'Vắng không phép';
                    } else {
                        statusClass = 'text-muted';
                        statusText = item.trangThai;
                    }

                    html += `<tr>
                        <td>${item.ngayHoc}</td>
                        <td>${item.lopHoc}</td>
                        <td class="${statusClass}">${statusText}</td>
                    </tr>`;
                });

                html += '</tbody></table></div>';
                container.innerHTML = html || '<p class="text-center">Chưa có lịch sử điểm danh</p>';

                // Update stats
                const presentEl = document.getElementById('present-count');
                if (presentEl) presentEl.innerText = presentCount;

                const lateEl = document.getElementById('late-count');
                if (lateEl) lateEl.innerText = lateCount;

                const absentEl = document.getElementById('absent-count');
                if (absentEl) absentEl.innerText = absentCount;

                const total = presentCount + lateCount + absentCount;
                const percent = total > 0 ? Math.round((presentCount / total) * 100) : 0;

                const percentEl = document.getElementById('attendance-percent');
                if (percentEl) percentEl.innerText = percent + '%';

                const barEl = document.getElementById('attendance-bar');
                if (barEl) barEl.style.width = percent + '%';
            }
        }
    } catch(e) {}
}

async function initTuition() {
    try {
        const res = await apiCall('/HocVienPortal/tuition');
        if (res.success && res.data.success) {
            const tbody = document.getElementById('tuitionTableBody');
            if (tbody) {
                let html = '';
                res.data.data.forEach(item => {
                    let badgeClass = item.trangThaiThanhToan === 'DaThanhToan' ? 'badge bg-success' : 'badge bg-danger';
                    let statusText = item.trangThaiThanhToan === 'DaThanhToan' ? 'Đã TT' : 'Chưa TT';
                    let ngayThanhToan = item.ngayThanhToan ? new Date(item.ngayThanhToan).toLocaleDateString('vi-VN') : '-';
                    html += `<tr>
                        <td>${item.loai}</td>
                        <td>${item.noiDung}</td>
                        <td>${item.soTien.toLocaleString()} VND</td>
                        <td><span class="${badgeClass}">${statusText}</span></td>
                        <td>${ngayThanhToan}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="5" class="text-center">Không có dữ liệu học phí</td></tr>';
            }
        }
    } catch(e) {}
}

async function initPayment() {
    try {
        const res = await apiCall('/HocVienPortal/tuition');
        if (res.success && res.data.success) {
            const tbody = document.getElementById('payment-tbody');
            if (tbody) {
                let html = '';
                let total = 0;
                res.data.data.forEach((item, index) => {
                    if (item.trangThaiThanhToan !== 'DaThanhToan') {
                        total += item.soTien;
                        const loaiThanhToan = item.loai === 'Lớp học' ? 'LopHoc' : 'KemRieng';
                        html += `<tr data-ma-dangky="${item.maDangKy || ''}" data-loai="${loaiThanhToan}" data-so-tien="${item.soTien}">
                            <td>${item.noiDung}</td>
                            <td>${item.loai}</td>
                            <td class="text-end fw-bold">${item.soTien.toLocaleString()} đ</td>
                            <td class="text-end">
                                <input type="checkbox" class="payment-checkbox" data-index="${index}" checked>
                            </td>
                        </tr>`;
                    }
                });
                tbody.innerHTML = html || '<tr><td colspan="4" class="text-center">Không có khoản nào cần thanh toán</td></tr>';
                
                const totalEl = document.getElementById('payment-total');
                if (totalEl) totalEl.innerText = total.toLocaleString() + ' đ';

                // Add checkbox change listeners
                const checkboxes = document.querySelectorAll('.payment-checkbox');
                checkboxes.forEach(cb => {
                    cb.addEventListener('change', updatePaymentTotal);
                });
            }
        }
    } catch(e) {}
}

function updatePaymentTotal() {
    const checkboxes = document.querySelectorAll('.payment-checkbox:checked');
    let total = 0;
    checkboxes.forEach(cb => {
        const row = cb.closest('tr');
        const soTien = parseInt(row.dataset.soTien);
        total += soTien;
    });
    const totalEl = document.getElementById('payment-total');
    if (totalEl) totalEl.innerText = total.toLocaleString() + ' đ';
}

window.processPayment = async function() {
    const checkboxes = document.querySelectorAll('.payment-checkbox:checked');
    if (checkboxes.length === 0) return toast('Vui lòng chọn ít nhất một khoản để thanh toán', 'warning');

    const paymentMethod = document.querySelector('input[name="paymentMethod"]:checked')?.value;
    if (!paymentMethod) return toast('Vui lòng chọn phương thức thanh toán', 'warning');

    if (!confirm(`Xác nhận thanh toán ${checkboxes.length} khoản bằng ${paymentMethod}?`)) return;

    try {
        // Process each payment
        let successCount = 0;
        for (const cb of checkboxes) {
            const row = cb.closest('tr');
            const maDangKy = row.dataset.maDangKy;
            const loaiThanhToan = row.dataset.loai;
            const soTien = row.dataset.soTien;

            const res = await apiCall('/HocVienPortal/pay', 'POST', {
                maDangKy: parseInt(maDangKy),
                loaiThanhToan: loaiThanhToan,
                phuongThucThanhToan: paymentMethod
            });

            if (res.success && res.data.success) {
                successCount++;
            }
        }

        if (successCount === checkboxes.length) {
            toast('Thanh toán thành công tất cả khoản', 'success');
            initPayment(); // Refresh
        } else if (successCount > 0) {
            toast(`Thanh toán thành công ${successCount}/${checkboxes.length} khoản`, 'warning');
            initPayment(); // Refresh
        } else {
            toast('Thanh toán thất bại', 'error');
        }
    } catch(e) { toast('Lỗi hệ thống', 'error'); }
}

async function initExams() {
    try {
        const exRes = await apiCall('/HocVienPortal/exams');
        if (exRes.success && exRes.data.success) {
            const tbody = document.getElementById('examTableBody');
            if (tbody) {
                let html = '';
                exRes.data.data.forEach(item => {
                    let badgeClass = item.daDat ? 'badge bg-success' : 'badge bg-danger';
                    let ngayThi = item.ngayThi ? new Date(item.ngayThi).toLocaleDateString('vi-VN') : '-';
                    html += `<tr>
                        <td>${ngayThi}</td>
                        <td>${item.diemSo || 'N/A'}</td>
                        <td><span class="${badgeClass}">${item.daDat ? 'Đạt' : 'Không đạt'}</span></td>
                        <td>${item.capDaiMoi || '-'}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="4" class="text-center">Chưa tham gia kỳ thi nào</td></tr>';
            }
        }
    } catch(e) {}
}

async function initNotifications() {
    try {
        const res = await apiCall('/HocVienPortal/notifications');
        if (res.success && res.data.success) {
            const container = document.getElementById('notificationsContainer');
            if (container) {
                let html = '';
                res.data.data.forEach((item, index) => {
                    html += `
                        <div class="notification-card">
                            <div class="notification-header" onclick="toggleNotification(${index})" style="cursor: pointer;">
                                <div class="notification-left">
                                    <div class="notification-icon">
                                        <i class="fa-solid fa-bell"></i>
                                    </div>
                                    <div>
                                        <h3>${item.tieuDe}</h3>
                                        <small><i class="fa-solid fa-clock"></i> ${new Date(item.ngayDang).toLocaleDateString('vi-VN')}</small>
                                    </div>
                                </div>
                                <i class="fa-solid fa-chevron-down arrow" id="arrow-${index}"></i>
                            </div>
                            <div class="notification-body" id="notification-body-${index}" style="display: none;">
                                <p>${item.noiDung}</p>
                            </div>
                        </div>
                    `;
                });
                container.innerHTML = html || '<p class="text-center">Không có thông báo nào</p>';
            }
        }
    } catch(e) {}
}

window.toggleNotification = function(index) {
    const body = document.getElementById(`notification-body-${index}`);
    const arrow = document.getElementById(`arrow-${index}`);
    if (body) {
        if (body.style.display === 'none') {
            body.style.display = 'block';
            arrow.classList.remove('fa-chevron-down');
            arrow.classList.add('fa-chevron-up');
        } else {
            body.style.display = 'none';
            arrow.classList.remove('fa-chevron-up');
            arrow.classList.add('fa-chevron-down');
        }
    }
}

async function initAvailableClasses() {
    try {
        // Load available classes
        const res = await apiCall('/HocVienPortal/classes-available');
        if (res.success && res.data.success) {
            const container = document.getElementById('classes-container');
            if (container) {
                let html = '';
                res.data.data.forEach(c => {
                    const availableSlots = c.soLuongToiDa - c.soLuongDaDangKy;
                    html += `<div class="col-md-6 mb-4">
                        <div class="card p-4 shadow-sm border-0" style="border-radius: 12px;">
                            <h4 class="mb-3">${c.khoaHoc}</h4>
                            <p><strong>Lịch học:</strong> ${c.lichHoc}</p>
                            <p><strong>HLV:</strong> ${c.huanLuyenVien}</p>
                            <p><strong>Học phí:</strong> ${c.hocPhi.toLocaleString()} VNĐ / tháng</p>
                            <p><strong>Yêu cầu cấp đai:</strong> ${c.yeuCauCapDai}</p>
                            <p><strong>Trạng thái:</strong> ${availableSlots > 0 ? `Còn ${availableSlots} chỗ` : 'Đã đầy'}</p>
                            <button class="btn btn-primary mt-2" ${availableSlots <= 0 ? 'disabled' : ''} onclick="enrollClass(${c.maLop})">
                                ${availableSlots > 0 ? 'Đăng ký ngay' : 'Đã đầy'}
                            </button>
                        </div>
                    </div>`;
                });
                container.innerHTML = html || '<p class="text-center">Không có lớp nào khả dụng</p>';
            }
        }

        // Load private packages
        const pkgRes = await apiCall('/HocVienPortal/private-packages-available');
        if (pkgRes.success && pkgRes.data.success) {
            const packageSelect = document.getElementById('package-select');
            if (packageSelect) {
                let html = '<option value="">-- Chọn gói --</option>';
                pkgRes.data.data.packages.forEach(p => {
                    html += `<option value="${p.maGoi}">${p.tenGoi} (${p.soBuoi} buổi) - ${p.hocPhi.toLocaleString()} VNĐ</option>`;
                });
                packageSelect.innerHTML = html;
            }

            const trainerSelect = document.getElementById('trainer-select');
            if (trainerSelect) {
                let html = '<option value="">-- Để trung tâm phân công --</option>';
                pkgRes.data.data.trainers.forEach(t => {
                    html += `<option value="${t.maHlv}">${t.hoTen} (${t.chuyenMon})</option>`;
                });
                trainerSelect.innerHTML = html;
            }
        }
    } catch(e) {}
}

window.enrollClass = async function(maLop) {
    if (!confirm('Xác nhận đăng ký lớp này?')) return;
    try {
        const res = await apiCall('/HocVienPortal/enroll', 'POST', { maLop });
        if (res.success && res.data.success) {
            toast('Đăng ký lớp thành công', 'success');
            initAvailableClasses(); // Refresh
        } else {
            toast(res.data?.message || 'Đăng ký thất bại', 'error');
        }
    } catch(e) { toast('Lỗi hệ thống', 'error'); }
}

window.bookPrivateTraining = async function() {
    const maGoi = document.getElementById('package-select')?.value;
    const maHlv = document.getElementById('trainer-select')?.value;

    if (!maGoi) return toast('Vui lòng chọn gói huấn luyện', 'warning');

    if (!confirm('Xác nhận đăng ký gói kèm riêng này?')) return;

    try {
        const res = await apiCall('/HocVienPortal/book-private', 'POST', { 
            maGoi: parseInt(maGoi),
            maHlv: maHlv ? parseInt(maHlv) : null
        });
        if (res.success && res.data.success) {
            toast('Đăng ký gói kèm riêng thành công', 'success');
            // Reset form
            document.getElementById('package-select').value = '';
            document.getElementById('trainer-select').value = '';
        } else {
            toast(res.data?.message || 'Đăng ký thất bại', 'error');
        }
    } catch(e) { toast('Lỗi hệ thống', 'error'); }
}

async function initAvailableEvents() {
    try {
        const res = await apiCall('/HocVienPortal/events-available');
        if (res.success && res.data.success) {
            const container = document.getElementById('events-container');
            if (container) {
                let html = '';
                res.data.data.forEach(e => {
                    const ngayDang = new Date(e.ngayDang);
                    const isAvailable = ngayDang >= new Date();
                    html += `<div class="col-md-12 mb-4">
                        <div class="card p-4 shadow-sm border-0 ${isAvailable ? '' : 'bg-light'}" style="border-radius: 12px; border-left: 5px solid ${isAvailable ? 'var(--accent-color)' : '#6c757d'} !important;">
                            <div class="d-flex justify-content-between align-items-center">
                                <h3 class="mb-1 ${isAvailable ? '' : 'text-muted'}">${e.tieuDe}</h3>
                                <span class="badge ${isAvailable ? 'bg-success' : 'bg-secondary'} px-3 py-2">${isAvailable ? 'Đang mở đăng ký' : 'Đã đóng'}</span>
                            </div>
                            <p class="text-muted"><i class="fa-solid fa-calendar-days me-2"></i> ${ngayDang.toLocaleDateString('vi-VN')}</p>
                            <p class="mt-2 ${isAvailable ? '' : 'text-muted'}">${e.noiDung}</p>
                            <div class="mt-3">
                                <button class="btn ${isAvailable ? 'btn-primary' : 'btn-secondary'} px-4 py-2 fw-bold" ${!isAvailable ? 'disabled' : ''} onclick="registerEvent(${e.maThongBao})">
                                    <i class="fa-solid fa-user-plus me-2"></i> ${isAvailable ? 'Đăng ký tham gia' : 'Đã kết thúc đăng ký'}
                                </button>
                            </div>
                        </div>
                    </div>`;
                });
                container.innerHTML = html || '<p class="text-center">Không có sự kiện nào khả dụng</p>';
            }
        }
    } catch(e) {}
}

window.registerEvent = async function(maThongBao) {
    if (!confirm('Xác nhận đăng ký sự kiện này?')) return;
    try {
        const res = await apiCall('/HocVienPortal/register-event', 'POST', { maThongBao });
        if (res.success && res.data.success) {
            toast('Đăng ký sự kiện thành công', 'success');
            initAvailableEvents(); // Refresh
        } else {
            toast(res.data?.message || 'Đăng ký thất bại', 'error');
        }
    } catch(e) { toast('Lỗi hệ thống', 'error'); }
}

async function initSettings() {
    const btn = document.getElementById('btnChangePassword');
    if (btn) {
        btn.addEventListener('click', async () => {
            const cur = document.getElementById('currentPassword')?.value;
            const newPwd = document.getElementById('newPassword')?.value;
            const confirm = document.getElementById('confirmPassword')?.value;

            if (!cur || !newPwd || !confirm) return toast('Vui lòng điền đủ thông tin', 'warning');
            if (newPwd !== confirm) return toast('Mật khẩu mới không khớp', 'error');

            try {
                const res = await apiCall('/HocVienPortal/change-password', 'PUT', { currentPassword: cur, newPassword: newPwd });
                if (res.success && res.data.success) {
                    toast('Đổi mật khẩu thành công', 'success');
                    document.getElementById('currentPassword').value = '';
                    document.getElementById('newPassword').value = '';
                    document.getElementById('confirmPassword').value = '';
                } else {
                    toast(res.data?.message || 'Lỗi đổi mật khẩu', 'error');
                }
            } catch(e) { toast('Lỗi hệ thống', 'error'); }
        });
    }
}

// Toast helper
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
