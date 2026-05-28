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
    } else if (path.includes('hocphi.html') || path.includes('thanhtoan.html')) {
        await initTuition();
    } else if (path.includes('thangdai.html')) {
        await initExams();
    } else if (path.includes('thongbao.html')) {
        await initNotifications();
    } else if (path.includes('dangkylop.html')) {
        await initAvailableClasses();
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
    
    // Assuming inputs have specific IDs, we'll try to map common ones.
    const mappings = {
        'hoTen': profileData.tenHocVien,
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

    const saveBtn = document.getElementById('btnSaveProfile');
    if (saveBtn) {
        saveBtn.addEventListener('click', async () => {
            const sdt = document.getElementById('sdt')?.value;
            const diaChi = document.getElementById('diaChi')?.value;
            try {
                const res = await apiCall('/HocVienPortal/profile', 'PUT', { SoDienThoai: sdt, DiaChi: diaChi });
                if (res.success && res.data.success) {
                    toast('Cập nhật hồ sơ thành công', 'success');
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
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                schRes.data.data.forEach(item => {
                    html += `<tr>
                        <td>${item.loai}</td>
                        <td>${item.lich}</td>
                        <td>${item.ten}</td>
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
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                attRes.data.data.forEach(item => {
                    html += `<tr>
                        <td>${item.ngayHoc}</td>
                        <td>${item.lopHoc}</td>
                        <td>${item.trangThai}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="3" class="text-center">Chưa có lịch sử điểm danh</td></tr>';
            }
        }
    } catch(e) {}
}

async function initTuition() {
    try {
        const res = await apiCall('/HocVienPortal/tuition');
        if (res.success && res.data.success) {
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                res.data.data.forEach(item => {
                    let badgeClass = item.trangThaiThanhToan === 'DaThanhToan' ? 'badge bg-success' : 'badge bg-danger';
                    let statusText = item.trangThaiThanhToan === 'DaThanhToan' ? 'Đã TT' : 'Chưa TT';
                    html += `<tr>
                        <td>${item.noiDung}</td>
                        <td>${item.soTien.toLocaleString()} VND</td>
                        <td><span class="${badgeClass}">${statusText}</span></td>
                        <td>${item.ngayThanhToan || '-'}</td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="4" class="text-center">Không có dữ liệu học phí</td></tr>';
            }
        }
    } catch(e) {}
}

async function initExams() {
    try {
        const exRes = await apiCall('/HocVienPortal/exams');
        if (exRes.success && exRes.data.success) {
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                exRes.data.data.forEach(item => {
                    let badgeClass = item.daDat ? 'badge bg-success' : 'badge bg-danger';
                    html += `<tr>
                        <td>${item.tenKyThi}</td>
                        <td>${item.ngayThi}</td>
                        <td>${item.diemSo}</td>
                        <td><span class="${badgeClass}">${item.daDat ? 'Đạt' : 'Không đạt'}</span></td>
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
            const container = document.querySelector('.notifications-list') || document.querySelector('.main');
            if (container) {
                let html = '<h2 class="mb-4">Thông báo</h2>';
                res.data.data.forEach(item => {
                    html += `
                        <div class="card bg-dark text-white mb-3" style="border: 1px solid #333;">
                            <div class="card-body">
                                <h5 class="card-title text-danger">${item.tieuDe}</h5>
                                <h6 class="card-subtitle mb-2 text-muted">${new Date(item.ngayDang).toLocaleDateString('vi-VN')}</h6>
                                <p class="card-text">${item.noiDung}</p>
                            </div>
                        </div>
                    `;
                });
                // If it's a specific list container, replace its innerHTML, otherwise append.
                if (document.querySelector('.notifications-list')) {
                    document.querySelector('.notifications-list').innerHTML = html;
                } else if (!html.includes('table')) { // simple fallback
                    // create a container
                    const div = document.createElement('div');
                    div.className = 'notifications-list mt-4';
                    div.innerHTML = html;
                    container.appendChild(div);
                }
            }
        }
    } catch(e) {}
}

async function initAvailableClasses() {
    try {
        const res = await apiCall('/HocVienPortal/classes-available');
        if (res.success && res.data.success) {
            const tbody = document.querySelector('.table tbody');
            if (tbody) {
                let html = '';
                res.data.data.forEach(c => {
                    html += `<tr>
                        <td>${c.khoaHoc}</td>
                        <td>${c.lichHoc}</td>
                        <td>${c.huanLuyenVien}</td>
                        <td>${c.hocPhi.toLocaleString()} VND</td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="enrollClass(${c.maLop})">Đăng ký</button>
                        </td>
                    </tr>`;
                });
                tbody.innerHTML = html || '<tr><td colspan="5" class="text-center">Không có lớp nào khả dụng</td></tr>';
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
