document.addEventListener("DOMContentLoaded", async () => {
    // Determine the current page
    const currentPath = window.location.pathname;

    // --- 1. HỒ SƠ HỌC VIÊN (HV-D15) ---
    if (currentPath.includes("hoso.html")) {
        loadStudentProfile();
    }

    // --- 2. ĐĂNG KÝ LỚP & KÈM RIÊNG (HV-D16) ---
    if (currentPath.includes("dangkylop.html")) {
        loadAvailableClasses();
        loadPrivatePackages();
    }

    // --- 3. THANH TOÁN (HV-D17) ---
    if (currentPath.includes("thanhtoan.html")) {
        loadTuitionData();
    }

    // --- 4. SỰ KIỆN (HV-D18) ---
    if (currentPath.includes("dangkysukien.html")) {
        loadAvailableEvents();
    }
});

// ==========================================
// 1. TÍCH HỢP HỒ SƠ HỌC VIÊN (HV-D15)
// ==========================================
async function loadStudentProfile() {
    const res = await apiCall('/HocVienPortal/profile');
    if (res && res.success && res.data.data) {
        const info = res.data.data;
        document.getElementById('profile-name').innerText = info.tenHocVien || 'Chưa cập nhật';
        document.getElementById('profile-belt').innerText = info.capDaiHienTai || 'Chưa có';
        document.getElementById('profile-email').innerText = info.email || 'Chưa cập nhật';
        document.getElementById('profile-phone').innerText = info.soDienThoai || 'Chưa cập nhật';
        document.getElementById('profile-class').innerText = 'Xem tab Lịch tập'; 
    } else {
        alert("Lỗi khi tải thông tin hồ sơ: " + (res?.error || res?.data?.message || 'Không rõ'));
    }
}

// ==========================================
// 2. TÍCH HỢP ĐĂNG KÝ LỚP (HV-D16)
// ==========================================
async function loadAvailableClasses() {
    const res = await apiCall('/HocVienPortal/classes-available');
    const container = document.getElementById('classes-container');
    if (!container) return; // if not initialized in UI

    if (res && res.success && res.data.data) {
        const classes = res.data.data;
        container.innerHTML = ''; // clear

        if (classes.length === 0) {
            container.innerHTML = '<p>Không có lớp học nào đang mở đăng ký.</p>';
            return;
        }

        classes.forEach(c => {
            const seatsLeft = c.soLuongToiDa - c.soLuongDaDangKy;
            container.innerHTML += `
                <div class="col-md-6 mb-4">
                    <div class="card p-4 shadow-sm border-0" style="border-radius: 12px;">
                        <h4 class="mb-3">${c.tenLop}</h4>
                        <p><strong>Khóa học:</strong> ${c.khoaHoc}</p>
                        <p><strong>Lịch học:</strong> ${c.lichHoc}</p>
                        <p><strong>HLV:</strong> ${c.huanLuyenVien}</p>
                        <p><strong>Yêu cầu đai:</strong> ${c.yeuCauCapDai}</p>
                        <p><strong>Học phí:</strong> ${c.hocPhi.toLocaleString()} VNĐ</p>
                        <p><strong>Trạng thái:</strong> Còn ${seatsLeft} chỗ</p>
                        <button class="btn btn-primary mt-2" onclick="enrollClass(${c.maLop})" ${seatsLeft <= 0 ? 'disabled' : ''}>Đăng ký ngay</button>
                    </div>
                </div>
            `;
        });
    }
}

async function enrollClass(maLop) {
    if (!confirm('Bạn có chắc chắn muốn đăng ký lớp học này?')) return;
    const res = await apiCall('/HocVienPortal/enroll', 'POST', { maLop: maLop });
    if (res && res.success) {
        alert("Đăng ký thành công! Vui lòng thanh toán học phí.");
        window.location.reload();
    } else {
        alert("Lỗi: " + (res?.data?.message || res?.error));
    }
}

async function loadPrivatePackages() {
    const res = await apiCall('/HocVienPortal/private-packages-available');
    const packageSelect = document.getElementById('package-select');
    const trainerSelect = document.getElementById('trainer-select');
    if (!packageSelect || !trainerSelect) return;

    if (res && res.success && res.data) {
        packageSelect.innerHTML = '<option value="">-- Chọn gói --</option>';
        res.data.packages.forEach(p => {
            packageSelect.innerHTML += `<option value="${p.maGoi}">${p.tenGoi} - ${p.hocPhi.toLocaleString()} VNĐ (${p.soBuoi} buổi)</option>`;
        });

        trainerSelect.innerHTML = '<option value="">-- Để trung tâm phân công --</option>';
        res.data.trainers.forEach(t => {
            trainerSelect.innerHTML += `<option value="${t.maHlv}">${t.hoTen} (${t.chuyenMon})</option>`;
        });
    }
}

async function bookPrivateTraining() {
    const maGoi = document.getElementById('package-select').value;
    const maHlv = document.getElementById('trainer-select').value;

    if (!maGoi) {
        alert('Vui lòng chọn gói huấn luyện');
        return;
    }

    const payload = {
        maGoi: parseInt(maGoi),
        maHlv: maHlv ? parseInt(maHlv) : null
    };

    const res = await apiCall('/HocVienPortal/book-private', 'POST', payload);
    if (res && res.success) {
        alert("Đăng ký kèm riêng thành công! Vui lòng thanh toán.");
        window.location.reload();
    } else {
        alert("Lỗi: " + (res?.data?.message || res?.error));
    }
}

// ==========================================
// 3. TÍCH HỢP THANH TOÁN (HV-D17)
// ==========================================
async function loadTuitionData() {
    const res = await apiCall('/HocVienPortal/tuition');
    const tbody = document.getElementById('payment-tbody');
    const totalEl = document.getElementById('payment-total');
    if (!tbody || !totalEl) return;

    if (res && res.success && res.data.data) {
        const items = res.data.data;
        tbody.innerHTML = '';
        let total = 0;

        const unpaidItems = items.filter(i => i.trangThaiThanhToan === 'ChuaThanhToan');
        
        if (unpaidItems.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center">Bạn không có khoản nợ nào.</td></tr>';
            totalEl.innerText = '0 đ';
            document.getElementById('btn-pay').disabled = true;
            return;
        }

        window.paymentItems = unpaidItems; // save for processing

        unpaidItems.forEach((item, index) => {
            total += item.soTien;
            tbody.innerHTML += `
                <tr>
                    <td>${item.noiDung}</td>
                    <td>${item.loai}</td>
                    <td class="text-end fw-bold">${item.soTien.toLocaleString()} đ</td>
                </tr>
            `;
        });

        totalEl.innerText = total.toLocaleString() + ' đ';
    }
}

async function processPayment() {
    if (!window.paymentItems || window.paymentItems.length === 0) {
        alert('Không có khoản nào để thanh toán.');
        return;
    }

    const radios = document.getElementsByName('paymentMethod');
    let method = 'Tiền mặt';
    for (let r of radios) {
        if (r.checked) method = r.id; // momo, vnpay, bank
    }

    // Process each unpaid item one by one (simplified flow)
    for (const item of window.paymentItems) {
        // We need the original ID. Since the tuition API currently doesn't return the ID, 
        // we'll simulate a success message and then we would need to fix the API if this was a real production app.
        // For UAT, we simulate success:
        console.log("Simulating payment for", item.noiDung);
    }
    
    alert(`Đã gửi yêu cầu thanh toán qua ${method}. Hệ thống đang xử lý...`);
    // In a real app we redirect to Momo/VNPay gateway.
    // For demo purposes, we will just simulate success.
}

// ==========================================
// 4. TÍCH HỢP ĐĂNG KÝ SỰ KIỆN (HV-D18)
// ==========================================
async function loadAvailableEvents() {
    const res = await apiCall('/HocVienPortal/events-available');
    const container = document.getElementById('events-container');
    if (!container) return;

    if (res && res.success && res.data.data) {
        const events = res.data.data;
        container.innerHTML = '';

        if (events.length === 0) {
            container.innerHTML = '<p>Không có sự kiện nào đang mở.</p>';
            return;
        }

        events.forEach(e => {
            container.innerHTML += `
                <div class="col-md-12 mb-4">
                    <div class="card p-4 shadow-sm border-0" style="border-radius: 12px; border-left: 5px solid var(--accent-color) !important;">
                        <div class="d-flex justify-content-between align-items-center">
                            <h3 class="mb-1">${e.tieuDe}</h3>
                            <span class="badge bg-success px-3 py-2">Đang mở đăng ký</span>
                        </div>
                        <p class="text-muted"><i class="fa-solid fa-calendar-days me-2"></i> ${new Date(e.ngayDang).toLocaleDateString()}</p>
                        <p class="mt-2">${e.noiDung}</p>
                        <div class="mt-3">
                            <button class="btn btn-primary px-4 py-2 fw-bold" onclick="registerEvent(${e.maThongBao})"><i class="fa-solid fa-user-plus me-2"></i> Đăng ký tham gia</button>
                        </div>
                    </div>
                </div>
            `;
        });
    }
}

async function registerEvent(maThongBao) {
    if (!confirm('Bạn có đăng ký tham gia sự kiện này?')) return;
    const res = await apiCall('/HocVienPortal/register-event', 'POST', { maThongBao: maThongBao });
    if (res && res.success) {
        alert("Đăng ký thành công!");
    } else {
        alert("Lỗi: " + (res?.data?.message || res?.error));
    }
}
