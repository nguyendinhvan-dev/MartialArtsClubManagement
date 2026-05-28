document.addEventListener('DOMContentLoaded', () => {
    // Check authorization first - allow Admin, QuanTri, or QuanTriVien roles
    if (!checkAuth(['Admin', 'QuanTri', 'QuanTriVien'])) {
        return;
    }

    // Load dashboard stats if we are on the dashboard
    if (window.location.pathname.includes('/admin/dashboard.html')) {
        loadDashboardStats();
        loadHocVienList();
    }

    // Load HocVien page
    if (window.location.pathname.includes('/admin/hocvien.html')) {
        loadHocVienList();
    }

    // Load HuanLuyenVien page
    if (window.location.pathname.includes('/admin/huanluyenvien.html')) {
        loadHuanLuyenVienList();
    }

    // Load LopHoc page
    if (window.location.pathname.includes('/admin/lophoc.html')) {
        loadLopHocList();
    }

    // Load DiemDanh page
    if (window.location.pathname.includes('/admin/diemdanh.html')) {
        loadDiemDanhList();
    }

    // Load HocPhi page
    if (window.location.pathname.includes('/admin/hocphi.html')) {
        loadHocPhiList();
    }

    // Load ThangDai page
    if (window.location.pathname.includes('/admin/thangdai.html')) {
        loadThangDaiList();
    }

    // Load TaiKhoan page
    if (window.location.pathname.includes('/admin/taikhoan.html')) {
        loadTaiKhoanList();
    }
});

async function loadDashboardStats() {
    try {
        const result = await apiCall('/admin/dashboard/stats', 'GET');
        
        if (result && result.success && result.data) {
            const stats = result.data;
            
            // Find stat elements (assuming they are in order: HocVien, HLV, Lop, DoanhThu)
            const statElements = document.querySelectorAll('.stat-num');
            if (statElements.length >= 4) {
                // Animate or set text
                statElements[0].textContent = stats.tongHocVien || stats.TongHocVien;
                statElements[1].textContent = stats.tongHuanLuyenVien || stats.TongHuanLuyenVien;
                statElements[2].textContent = stats.tongLopHoc || stats.TongLopHoc;
                
                let revenue = stats.doanhThuThangNay || stats.DoanhThuThangNay || 0;
                let revenueText = revenue.toLocaleString('vi-VN') + ' đ';
                if (revenue >= 1000000) {
                    revenueText = (revenue / 1000000).toFixed(1) + 'M';
                }
                statElements[3].textContent = revenueText;
            }
        } else {
            console.error('Failed to load dashboard stats:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading stats:', error);
    }
}

// ==================== HỌC VIÊN ====================
async function loadHocVienList() {
    try {
        const result = await apiCall('/admin/hocvien', 'GET');
        
        if (result && result.success && result.data) {
            renderHocVienTable(result.data);
        } else {
            console.error('Failed to load HocVien list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading HocVien list:', error);
    }
}

function renderHocVienTable(hocViens) {
    const tbody = document.getElementById('tbodyHocVien');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    hocViens.forEach((hv, index) => {
        const row = document.createElement('tr');
        row.setAttribute('data-dai', getCapDaiName(hv.maCapDai));
        row.setAttribute('data-status', hv.trangThai === 1 ? 'active' : 'inactive');
        
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${hv.hoTen}</div><div class="td-sub">${hv.email}</div></td>
            <td><div class="td-sub">${hv.soDienThoai}</div></td>
            <td><span class="belt ${getBeltClass(hv.maCapDai)}">${getCapDaiName(hv.maCapDai)}</span></td>
            <td><div class="td-sub">${formatDate(hv.ngayVaoClb)}</div></td>
            <td><span class="status ${hv.trangThai === 1 ? 'status-active' : 'status-inactive'}"><span class="status-dot"></span>${hv.trangThai === 1 ? 'Đang học' : 'Nghỉ học'}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-view" title="Xem" onclick="openViewHocVien(${hv.maHocVien})"><i class="fas fa-eye"></i></button>
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditHocVien(${hv.maHocVien})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('${hv.hoTen}', function(){ xoaHocVien(${hv.maHocVien}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

function getCapDaiName(maCapDai) {
    const capDaiMap = {
        1: 'Trắng',
        2: 'Vàng',
        3: 'Cam',
        4: 'Xanh lá',
        5: 'Xanh dương',
        6: 'Đỏ',
        7: 'Đen'
    };
    return capDaiMap[maCapDai] || 'Chưa xếp';
}

function getBeltClass(maCapDai) {
    const beltClassMap = {
        1: 'belt-white',
        2: 'belt-yellow',
        3: 'belt-orange',
        4: 'belt-green',
        5: 'belt-blue',
        6: 'belt-red',
        7: 'belt-black'
    };
    return beltClassMap[maCapDai] || '';
}

async function luuThemHocVien() {
    const hoTen = document.getElementById('them_HoTen').value;
    const email = document.getElementById('them_Email').value;
    const matKhau = document.getElementById('them_MatKhau').value;
    const xacNhan = document.getElementById('them_XacNhan').value;
    const sdt = document.getElementById('them_SDT').value;
    const ngaySinh = document.getElementById('them_NgaySinh').value;
    const gioiTinh = document.getElementById('them_GioiTinh').value;
    const capDai = document.getElementById('them_CapDai').value;
    const diaChi = document.getElementById('them_DiaChi').value;

    if (!hoTen || !email || !matKhau) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    if (matKhau !== xacNhan) {
        toast('Mật khẩu không khớp', 'error');
        return;
    }

    try {
        const data = {
            hoTen: hoTen,
            email: email,
            matKhau: matKhau,
            soDienThoai: sdt,
            ngaySinh: ngaySinh,
            gioiTinh: gioiTinh,
            maCapDai: parseInt(capDai) || null,
            diaChi: diaChi,
            trangThai: 1
        };

        const result = await apiCall('/admin/hocvien', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm học viên thành công', 'success');
            hideModal('modalThemHocVien');
            loadHocVienList();
            // Clear form
            document.getElementById('them_HoTen').value = '';
            document.getElementById('them_Email').value = '';
            document.getElementById('them_MatKhau').value = '';
            document.getElementById('them_XacNhan').value = '';
            document.getElementById('them_SDT').value = '';
            document.getElementById('them_NgaySinh').value = '';
            document.getElementById('them_GioiTinh').value = '';
            document.getElementById('them_CapDai').value = '';
            document.getElementById('them_DiaChi').value = '';
        } else {
            toast(result?.data?.message || 'Thêm học viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding HocVien:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditHocVien(maHocVien) {
    try {
        const result = await apiCall(`/admin/hocvien/${maHocVien}`, 'GET');
        
        if (result && result.success && result.data) {
            const hv = result.data;
            document.getElementById('sua_MaHV').value = hv.maHocVien;
            document.getElementById('sua_HoTen').value = hv.hoTen;
            document.getElementById('sua_Email').value = hv.email;
            document.getElementById('sua_SDT').value = hv.soDienThoai;
            document.getElementById('sua_NgaySinh').value = hv.ngaySinh;
            document.getElementById('sua_GioiTinh').value = hv.gioiTinh;
            document.getElementById('sua_CapDai').value = hv.maCapDai;
            document.getElementById('sua_DiaChi').value = hv.diaChi;
            document.getElementById('sua_TrangThai').value = hv.trangThai;
            showModal('modalSuaHocVien');
        } else {
            toast('Không thể tải thông tin học viên', 'error');
        }
    } catch (error) {
        console.error('Error loading HocVien detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaHocVien() {
    const maHocVien = document.getElementById('sua_MaHV').value;
    const hoTen = document.getElementById('sua_HoTen').value;
    const sdt = document.getElementById('sua_SDT').value;
    const ngaySinh = document.getElementById('sua_NgaySinh').value;
    const gioiTinh = document.getElementById('sua_GioiTinh').value;
    const capDai = document.getElementById('sua_CapDai').value;
    const diaChi = document.getElementById('sua_DiaChi').value;
    const trangThai = document.getElementById('sua_TrangThai').value;

    try {
        const data = {
            maHocVien: parseInt(maHocVien),
            hoTen: hoTen,
            soDienThoai: sdt,
            ngaySinh: ngaySinh,
            gioiTinh: gioiTinh,
            maCapDai: parseInt(capDai) || null,
            diaChi: diaChi,
            trangThai: parseInt(trangThai)
        };

        const result = await apiCall(`/admin/hocvien/${maHocVien}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật học viên thành công', 'success');
            hideModal('modalSuaHocVien');
            loadHocVienList();
        } else {
            toast(result?.data?.message || 'Cập nhật học viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating HocVien:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaHocVien(maHocVien) {
    try {
        const result = await apiCall(`/admin/hocvien/${maHocVien}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa học viên thành công', 'success');
            loadHocVienList();
        } else {
            toast(result?.data?.message || 'Xóa học viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting HocVien:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openViewHocVien(maHocVien) {
    try {
        const result = await apiCall(`/admin/hocvien/${maHocVien}`, 'GET');
        
        if (result && result.success && result.data) {
            const hv = result.data;
            document.getElementById('xem_Avatar').textContent = hv.hoTen.charAt(0).toUpperCase();
            document.getElementById('xem_TenHV').textContent = hv.hoTen;
            document.getElementById('xem_BeltBadge').innerHTML = `<span class="belt ${getBeltClass(hv.maCapDai)}">${getCapDaiName(hv.maCapDai)}</span>`;
            document.getElementById('xem_Email').textContent = hv.email;
            document.getElementById('xem_SDT').textContent = hv.soDienThoai;
            showModal('modalXemHocVien');
        } else {
            toast('Không thể tải thông tin học viên', 'error');
        }
    } catch (error) {
        console.error('Error loading HocVien detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

// ==================== HUẤN LUYỆN VIÊN ====================
async function loadHuanLuyenVienList() {
    try {
        const result = await apiCall('/admin/huanluyenvien', 'GET');
        
        if (result && result.success && result.data) {
            renderHuanLuyenVienTable(result.data);
        } else {
            console.error('Failed to load HuanLuyenVien list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading HuanLuyenVien list:', error);
    }
}

function renderHuanLuyenVienTable(hlvs) {
    const tbody = document.getElementById('tbodyHLV');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    hlvs.forEach((hlv, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${hlv.hoTen}</div><div class="td-sub">${hlv.email}</div></td>
            <td><div class="td-sub">${hlv.soDienThoai}</div></td>
            <td><div class="td-sub">${formatDate(hlv.ngayVaoClb)}</div></td>
            <td><span class="status ${hlv.trangThai === 1 ? 'status-active' : 'status-inactive'}"><span class="status-dot"></span>${hlv.trangThai === 1 ? 'Đang hoạt động' : 'Ngừng hoạt động'}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-view" title="Xem" onclick="openViewHLV(${hlv.maHlv})"><i class="fas fa-eye"></i></button>
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditHLV(${hlv.maHlv})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('${hlv.hoTen}', function(){ xoaHLV(${hlv.maHlv}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== LỚP HỌC ====================
async function loadLopHocList() {
    try {
        const result = await apiCall('/admin/lophoc', 'GET');
        
        if (result && result.success && result.data) {
            renderLopHocTable(result.data);
        } else {
            console.error('Failed to load LopHoc list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading LopHoc list:', error);
    }
}

function renderLopHocTable(lops) {
    const tbody = document.getElementById('tbodyLop');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    lops.forEach((lop, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${lop.tenLop}</div></td>
            <td><div class="td-sub">${lop.lichHoc}</div></td>
            <td><div class="td-sub">${lop.phongTap || 'Chưa xếp'}</div></td>
            <td><div class="td-sub">${lop.soLuongToiDa}</div></td>
            <td><div class="td-sub">${lop.hocPhi.toLocaleString('vi-VN')} đ</div></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-view" title="Xem" onclick="openViewLop(${lop.maLop})"><i class="fas fa-eye"></i></button>
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditLop(${lop.maLop})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('${lop.tenLop}', function(){ xoaLop(${lop.maLop}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== ĐIỂM DANH ====================
async function loadDiemDanhList() {
    try {
        const result = await apiCall('/admin/diemdanh', 'GET');
        
        if (result && result.success && result.data) {
            renderDiemDanhTable(result.data);
        } else {
            console.error('Failed to load DiemDanh list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading DiemDanh list:', error);
    }
}

function renderDiemDanhTable(diemDanhs) {
    const tbody = document.getElementById('tbodyDiemDanh');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    diemDanhs.forEach((dd, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${dd.hocVienHoTen}</div></td>
            <td><div class="td-sub">${dd.lopHocTen}</div></td>
            <td><div class="td-sub">${formatDate(dd.ngayHoc)}</div></td>
            <td><span class="status ${dd.trangThai === 'CoMat' ? 'status-active' : 'status-inactive'}"><span class="status-dot"></span>${dd.trangThai}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditDiemDanh(${dd.maDiemDanh})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('Điểm danh', function(){ xoaDiemDanh(${dd.maDiemDanh}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== HỌC PHÍ ====================
async function loadHocPhiList() {
    try {
        const result = await apiCall('/admin/hocphi', 'GET');
        
        if (result && result.success && result.data) {
            renderHocPhiTable(result.data);
        } else {
            console.error('Failed to load HocPhi list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading HocPhi list:', error);
    }
}

function renderHocPhiTable(hocPhis) {
    const tbody = document.getElementById('tbodyHocPhi');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    hocPhis.forEach((hp, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${hp.hocVienHoTen}</div></td>
            <td><div class="td-sub">${formatDate(hp.thang)}</div></td>
            <td><div class="td-sub">${hp.soTien.toLocaleString('vi-VN')} đ</div></td>
            <td><span class="status ${hp.trangThaiThanhToan === 'DaDong' ? 'status-paid' : 'status-unpaid'}"><span class="status-dot"></span>${hp.trangThaiThanhToan}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditHocPhi(${hp.maHocPhi})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('Học phí', function(){ xoaHocPhi(${hp.maHocPhi}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== THĂNG ĐAI ====================
async function loadThangDaiList() {
    try {
        const result = await apiCall('/admin/thangdai', 'GET');
        
        if (result && result.success && result.data) {
            renderThangDaiTable(result.data);
        } else {
            console.error('Failed to load ThangDai list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading ThangDai list:', error);
    }
}

function renderThangDaiTable(thangDais) {
    const tbody = document.getElementById('tbodyThangDai');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    thangDais.forEach((td, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${td.tenKyThi}</div></td>
            <td><div class="td-sub">${formatDate(td.ngayThi)}</div></td>
            <td><span class="status ${td.trangThai === 'DaKetThuc' ? 'status-active' : 'status-pending'}"><span class="status-dot"></span>${td.trangThai}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-view" title="Xem" onclick="openViewThangDai(${td.maKyThi})"><i class="fas fa-eye"></i></button>
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditThangDai(${td.maKyThi})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('${td.tenKyThi}', function(){ xoaThangDai(${td.maKyThi}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== TÀI KHOẢN ====================
async function loadTaiKhoanList() {
    try {
        const result = await apiCall('/admin/taikhoan', 'GET');
        
        if (result && result.success && result.data) {
            renderTaiKhoanTable(result.data);
        } else {
            console.error('Failed to load TaiKhoan list:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading TaiKhoan list:', error);
    }
}

function renderTaiKhoanTable(taiKhoans) {
    const tbody = document.getElementById('tbodyTaiKhoan');
    if (!tbody) return;

    tbody.innerHTML = '';
    
    taiKhoans.forEach((tk, index) => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td style="color:var(--text-muted);">${index + 1}</td>
            <td><div class="td-main">${tk.email}</div></td>
            <td><div class="td-sub">${tk.vaiTro}</div></td>
            <td><span class="status ${tk.trangThai === 1 ? 'status-active' : 'status-inactive'}"><span class="status-dot"></span>${tk.trangThai === 1 ? 'Hoạt động' : 'Khóa'}</span></td>
            <td style="text-align:center;white-space:nowrap;vertical-align:middle;"><div style="display:flex;gap:5px;justify-content:center;align-items:center;">
                <button class="action-btn btn-edit" title="Sửa" onclick="openEditTaiKhoan(${tk.maTaiKhoan})"><i class="fas fa-pen"></i></button>
                <button class="action-btn btn-delete" title="Xóa" onclick="openDeleteModal('${tk.email}', function(){ xoaTaiKhoan(${tk.maTaiKhoan}); })"><i class="fas fa-trash"></i></button>
            </div></td>
        `;
        tbody.appendChild(row);
    });
}

// ==================== UTILITY FUNCTIONS ====================
function formatDate(dateString) {
    if (!dateString) return '—';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN');
}

async function luuThemHLV() {
    const hoTen = document.getElementById('them_HoTenHLV').value;
    const email = document.getElementById('them_EmailHLV').value;
    const matKhau = document.getElementById('them_MatKhauHLV').value;
    const sdt = document.getElementById('them_SDT_HLV').value;
    const ngaySinh = document.getElementById('them_NgaySinhHLV').value;
    const gioiTinh = document.getElementById('them_GioiTinhHLV').value;
    const diaChi = document.getElementById('them_DiaChiHLV').value;

    if (!hoTen || !email || !matKhau) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            hoTen: hoTen,
            email: email,
            matKhau: matKhau,
            soDienThoai: sdt,
            ngaySinh: ngaySinh,
            gioiTinh: gioiTinh,
            diaChi: diaChi,
            trangThai: 1
        };

        const result = await apiCall('/admin/huanluyenvien', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm huấn luyện viên thành công', 'success');
            hideModal('modalThemHLV');
            loadHuanLuyenVienList();
        } else {
            toast(result?.data?.message || 'Thêm huấn luyện viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding HLV:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditHLV(maHlv) {
    try {
        const result = await apiCall(`/admin/huanluyenvien/${maHlv}`, 'GET');
        
        if (result && result.success && result.data) {
            const hlv = result.data;
            document.getElementById('sua_MaHLV').value = hlv.maHlv;
            document.getElementById('sua_HoTenHLV').value = hlv.hoTen;
            document.getElementById('sua_EmailHLV').value = hlv.email;
            document.getElementById('sua_SDT_HLV').value = hlv.soDienThoai;
            document.getElementById('sua_NgaySinhHLV').value = hlv.ngaySinh;
            document.getElementById('sua_GioiTinhHLV').value = hlv.gioiTinh;
            document.getElementById('sua_DiaChiHLV').value = hlv.diaChi;
            document.getElementById('sua_TrangThaiHLV').value = hlv.trangThai;
            showModal('modalSuaHLV');
        } else {
            toast('Không thể tải thông tin huấn luyện viên', 'error');
        }
    } catch (error) {
        console.error('Error loading HLV detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaHLV() {
    const maHlv = document.getElementById('sua_MaHLV').value;
    const hoTen = document.getElementById('sua_HoTenHLV').value;
    const sdt = document.getElementById('sua_SDT_HLV').value;
    const ngaySinh = document.getElementById('sua_NgaySinhHLV').value;
    const gioiTinh = document.getElementById('sua_GioiTinhHLV').value;
    const diaChi = document.getElementById('sua_DiaChiHLV').value;
    const trangThai = document.getElementById('sua_TrangThaiHLV').value;

    try {
        const data = {
            maHlv: parseInt(maHlv),
            hoTen: hoTen,
            soDienThoai: sdt,
            ngaySinh: ngaySinh,
            gioiTinh: gioiTinh,
            diaChi: diaChi,
            trangThai: parseInt(trangThai)
        };

        const result = await apiCall(`/admin/huanluyenvien/${maHlv}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật huấn luyện viên thành công', 'success');
            hideModal('modalSuaHLV');
            loadHuanLuyenVienList();
        } else {
            toast(result?.data?.message || 'Cập nhật huấn luyện viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating HLV:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaHLV(maHlv) {
    try {
        const result = await apiCall(`/admin/huanluyenvien/${maHlv}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa huấn luyện viên thành công', 'success');
            loadHuanLuyenVienList();
        } else {
            toast(result?.data?.message || 'Xóa huấn luyện viên thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting HLV:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openViewHLV(maHlv) {
    try {
        const result = await apiCall(`/admin/huanluyenvien/${maHlv}`, 'GET');
        
        if (result && result.success && result.data) {
            const hlv = result.data;
            document.getElementById('xem_AvatarHLV').textContent = hlv.hoTen.charAt(0).toUpperCase();
            document.getElementById('xem_TenHLV').textContent = hlv.hoTen;
            document.getElementById('xem_EmailHLV').textContent = hlv.email;
            document.getElementById('xem_SDT_HLV').textContent = hlv.soDienThoai;
            document.getElementById('xem_NgayVaoCLB').textContent = formatDate(hlv.ngayVaoClb);
            showModal('modalXemHLV');
        } else {
            toast('Không thể tải thông tin huấn luyện viên', 'error');
        }
    } catch (error) {
        console.error('Error loading HLV detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}
async function luuThemLop() {
    const tenLop = document.getElementById('them_TenLop').value;
    const maKhoaHoc = document.getElementById('them_KhoaHoc').value;
    const maCapDai = document.getElementById('them_CapDai').value;
    const maHlv = document.getElementById('them_HLV').value;
    const lichHoc = document.getElementById('them_LichHoc').value;
    const phongTap = document.getElementById('them_PhongTap').value;
    const hocPhi = document.getElementById('them_HocPhi').value;
    const soLuongToiDa = document.getElementById('them_SoLuong').value;

    if (!tenLop || !maKhoaHoc || !maCapDai || !maHlv || !lichHoc) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            tenLop: tenLop,
            maKhoaHoc: parseInt(maKhoaHoc),
            maCapDai: parseInt(maCapDai),
            maHlv: parseInt(maHlv),
            lichHoc: lichHoc,
            phongTap: phongTap,
            hocPhi: parseFloat(hocPhi) || 0,
            soLuongToiDa: parseInt(soLuongToiDa) || 30
        };

        const result = await apiCall('/admin/lophoc', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm lớp học thành công', 'success');
            hideModal('modalThemLop');
            loadLopHocList();
        } else {
            toast(result?.data?.message || 'Thêm lớp học thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding Lop:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditLop(maLop) {
    try {
        const result = await apiCall(`/admin/lophoc/${maLop}`, 'GET');
        
        if (result && result.success && result.data) {
            const lop = result.data;
            document.getElementById('sua_MaLop').value = lop.maLop;
            document.getElementById('sua_TenLop').value = lop.tenLop;
            document.getElementById('sua_KhoaHoc').value = lop.maKhoaHoc;
            document.getElementById('sua_CapDai').value = lop.maCapDai;
            document.getElementById('sua_HLV').value = lop.maHlv;
            document.getElementById('sua_LichHoc').value = lop.lichHoc;
            document.getElementById('sua_PhongTap').value = lop.phongTap;
            document.getElementById('sua_HocPhi').value = lop.hocPhi;
            document.getElementById('sua_SoLuong').value = lop.soLuongToiDa;
            showModal('modalSuaLop');
        } else {
            toast('Không thể tải thông tin lớp học', 'error');
        }
    } catch (error) {
        console.error('Error loading Lop detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaLop() {
    const maLop = document.getElementById('sua_MaLop').value;
    const tenLop = document.getElementById('sua_TenLop').value;
    const maKhoaHoc = document.getElementById('sua_KhoaHoc').value;
    const maCapDai = document.getElementById('sua_CapDai').value;
    const maHlv = document.getElementById('sua_HLV').value;
    const lichHoc = document.getElementById('sua_LichHoc').value;
    const phongTap = document.getElementById('sua_PhongTap').value;
    const hocPhi = document.getElementById('sua_HocPhi').value;
    const soLuongToiDa = document.getElementById('sua_SoLuong').value;

    try {
        const data = {
            maLop: parseInt(maLop),
            tenLop: tenLop,
            maKhoaHoc: parseInt(maKhoaHoc),
            maCapDai: parseInt(maCapDai),
            maHlv: parseInt(maHlv),
            lichHoc: lichHoc,
            phongTap: phongTap,
            hocPhi: parseFloat(hocPhi) || 0,
            soLuongToiDa: parseInt(soLuongToiDa) || 30
        };

        const result = await apiCall(`/admin/lophoc/${maLop}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật lớp học thành công', 'success');
            hideModal('modalSuaLop');
            loadLopHocList();
        } else {
            toast(result?.data?.message || 'Cập nhật lớp học thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating Lop:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaLop(maLop) {
    try {
        const result = await apiCall(`/admin/lophoc/${maLop}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa lớp học thành công', 'success');
            loadLopHocList();
        } else {
            toast(result?.data?.message || 'Xóa lớp học thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting Lop:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openViewLop(maLop) {
    try {
        const result = await apiCall(`/admin/lophoc/${maLop}`, 'GET');
        
        if (result && result.success && result.data) {
            const lop = result.data;
            document.getElementById('xem_TenLop').textContent = lop.tenLop;
            document.getElementById('xem_LichHoc').textContent = lop.lichHoc;
            document.getElementById('xem_PhongTap').textContent = lop.phongTap || 'Chưa xếp';
            document.getElementById('xem_HocPhi').textContent = lop.hocPhi.toLocaleString('vi-VN') + ' đ';
            document.getElementById('xem_SoLuong').textContent = lop.soLuongToiDa;
            showModal('modalXemLop');
        } else {
            toast('Không thể tải thông tin lớp học', 'error');
        }
    } catch (error) {
        console.error('Error loading Lop detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}
async function luuThemDiemDanh() {
    const maHocVien = document.getElementById('them_HocVien').value;
    const maLop = document.getElementById('them_Lop').value;
    const ngayHoc = document.getElementById('them_NgayHoc').value;
    const trangThai = document.getElementById('them_TrangThai').value;

    if (!maHocVien || !maLop || !ngayHoc) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            maHocVien: parseInt(maHocVien),
            maLop: parseInt(maLop),
            ngayHoc: ngayHoc,
            trangThai: trangThai
        };

        const result = await apiCall('/admin/diemdanh', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm điểm danh thành công', 'success');
            hideModal('modalThemDiemDanh');
            loadDiemDanhList();
        } else {
            toast(result?.data?.message || 'Thêm điểm danh thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding DiemDanh:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditDiemDanh(maDiemDanh) {
    try {
        const result = await apiCall(`/admin/diemdanh/${maDiemDanh}`, 'GET');
        
        if (result && result.success && result.data) {
            const dd = result.data;
            document.getElementById('sua_MaDiemDanh').value = dd.maDiemDanh;
            document.getElementById('sua_HocVien').value = dd.maHocVien;
            document.getElementById('sua_Lop').value = dd.maLop;
            document.getElementById('sua_NgayHoc').value = dd.ngayHoc;
            document.getElementById('sua_TrangThai').value = dd.trangThai;
            showModal('modalSuaDiemDanh');
        } else {
            toast('Không thể tải thông tin điểm danh', 'error');
        }
    } catch (error) {
        console.error('Error loading DiemDanh detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaDiemDanh() {
    const maDiemDanh = document.getElementById('sua_MaDiemDanh').value;
    const maHocVien = document.getElementById('sua_HocVien').value;
    const maLop = document.getElementById('sua_Lop').value;
    const ngayHoc = document.getElementById('sua_NgayHoc').value;
    const trangThai = document.getElementById('sua_TrangThai').value;

    try {
        const data = {
            maDiemDanh: parseInt(maDiemDanh),
            maHocVien: parseInt(maHocVien),
            maLop: parseInt(maLop),
            ngayHoc: ngayHoc,
            trangThai: trangThai
        };

        const result = await apiCall(`/admin/diemdanh/${maDiemDanh}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật điểm danh thành công', 'success');
            hideModal('modalSuaDiemDanh');
            loadDiemDanhList();
        } else {
            toast(result?.data?.message || 'Cập nhật điểm danh thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating DiemDanh:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaDiemDanh(maDiemDanh) {
    try {
        const result = await apiCall(`/admin/diemdanh/${maDiemDanh}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa điểm danh thành công', 'success');
            loadDiemDanhList();
        } else {
            toast(result?.data?.message || 'Xóa điểm danh thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting DiemDanh:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuThemHocPhi() {
    const maHocVien = document.getElementById('them_HocVienHP').value;
    const thang = document.getElementById('them_Thang').value;
    const soTien = document.getElementById('them_SoTien').value;
    const trangThaiThanhToan = document.getElementById('them_TrangThai').value;

    if (!maHocVien || !thang || !soTien) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            maHocVien: parseInt(maHocVien),
            thang: thang,
            soTien: parseFloat(soTien),
            trangThaiThanhToan: trangThaiThanhToan
        };

        const result = await apiCall('/admin/hocphi', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm học phí thành công', 'success');
            hideModal('modalThemHocPhi');
            loadHocPhiList();
        } else {
            toast(result?.data?.message || 'Thêm học phí thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding HocPhi:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditHocPhi(maHocPhi) {
    try {
        const result = await apiCall(`/admin/hocphi/${maHocPhi}`, 'GET');
        
        if (result && result.success && result.data) {
            const hp = result.data;
            document.getElementById('sua_MaHocPhi').value = hp.maHocPhi;
            document.getElementById('sua_HocVienHP').value = hp.maHocVien;
            document.getElementById('sua_Thang').value = hp.thang;
            document.getElementById('sua_SoTien').value = hp.soTien;
            document.getElementById('sua_TrangThai').value = hp.trangThaiThanhToan;
            showModal('modalSuaHocPhi');
        } else {
            toast('Không thể tải thông tin học phí', 'error');
        }
    } catch (error) {
        console.error('Error loading HocPhi detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaHocPhi() {
    const maHocPhi = document.getElementById('sua_MaHocPhi').value;
    const maHocVien = document.getElementById('sua_HocVienHP').value;
    const thang = document.getElementById('sua_Thang').value;
    const soTien = document.getElementById('sua_SoTien').value;
    const trangThaiThanhToan = document.getElementById('sua_TrangThai').value;

    try {
        const data = {
            maHocPhi: parseInt(maHocPhi),
            maHocVien: parseInt(maHocVien),
            thang: thang,
            soTien: parseFloat(soTien),
            trangThaiThanhToan: trangThaiThanhToan
        };

        const result = await apiCall(`/admin/hocphi/${maHocPhi}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật học phí thành công', 'success');
            hideModal('modalSuaHocPhi');
            loadHocPhiList();
        } else {
            toast(result?.data?.message || 'Cập nhật học phí thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating HocPhi:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaHocPhi(maHocPhi) {
    try {
        const result = await apiCall(`/admin/hocphi/${maHocPhi}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa học phí thành công', 'success');
            loadHocPhiList();
        } else {
            toast(result?.data?.message || 'Xóa học phí thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting HocPhi:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuThemThangDai() {
    const tenKyThi = document.getElementById('them_TenKyThi').value;
    const maKhoaHoc = document.getElementById('them_KhoaHocTD').value;
    const ngayThi = document.getElementById('them_NgayThi').value;
    const moTa = document.getElementById('them_MoTa').value;
    const trangThai = document.getElementById('them_TrangThaiTD').value;

    if (!tenKyThi || !maKhoaHoc || !ngayThi) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            tenKyThi: tenKyThi,
            maKhoaHoc: parseInt(maKhoaHoc),
            ngayThi: ngayThi,
            moTa: moTa,
            trangThai: trangThai
        };

        const result = await apiCall('/admin/thangdai', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm kỳ thi thành công', 'success');
            hideModal('modalThemThangDai');
            loadThangDaiList();
        } else {
            toast(result?.data?.message || 'Thêm kỳ thi thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding ThangDai:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditThangDai(maKyThi) {
    try {
        const result = await apiCall(`/admin/thangdai/${maKyThi}`, 'GET');
        
        if (result && result.success && result.data) {
            const td = result.data;
            document.getElementById('sua_MaKyThi').value = td.maKyThi;
            document.getElementById('sua_TenKyThi').value = td.tenKyThi;
            document.getElementById('sua_KhoaHocTD').value = td.maKhoaHoc;
            document.getElementById('sua_NgayThi').value = td.ngayThi;
            document.getElementById('sua_MoTa').value = td.moTa;
            document.getElementById('sua_TrangThaiTD').value = td.trangThai;
            showModal('modalSuaThangDai');
        } else {
            toast('Không thể tải thông tin kỳ thi', 'error');
        }
    } catch (error) {
        console.error('Error loading ThangDai detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaThangDai() {
    const maKyThi = document.getElementById('sua_MaKyThi').value;
    const tenKyThi = document.getElementById('sua_TenKyThi').value;
    const maKhoaHoc = document.getElementById('sua_KhoaHocTD').value;
    const ngayThi = document.getElementById('sua_NgayThi').value;
    const moTa = document.getElementById('sua_MoTa').value;
    const trangThai = document.getElementById('sua_TrangThaiTD').value;

    try {
        const data = {
            maKyThi: parseInt(maKyThi),
            tenKyThi: tenKyThi,
            maKhoaHoc: parseInt(maKhoaHoc),
            ngayThi: ngayThi,
            moTa: moTa,
            trangThai: trangThai
        };

        const result = await apiCall(`/admin/thangdai/${maKyThi}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật kỳ thi thành công', 'success');
            hideModal('modalSuaThangDai');
            loadThangDaiList();
        } else {
            toast(result?.data?.message || 'Cập nhật kỳ thi thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating ThangDai:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaThangDai(maKyThi) {
    try {
        const result = await apiCall(`/admin/thangdai/${maKyThi}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa kỳ thi thành công', 'success');
            loadThangDaiList();
        } else {
            toast(result?.data?.message || 'Xóa kỳ thi thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting ThangDai:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openViewThangDai(maKyThi) {
    try {
        const result = await apiCall(`/admin/thangdai/${maKyThi}`, 'GET');
        
        if (result && result.success && result.data) {
            const td = result.data;
            document.getElementById('xem_TenKyThi').textContent = td.tenKyThi;
            document.getElementById('xem_NgayThi').textContent = formatDate(td.ngayThi);
            document.getElementById('xem_MoTa').textContent = td.moTa || '—';
            document.getElementById('xem_TrangThai').textContent = td.trangThai;
            showModal('modalXemThangDai');
        } else {
            toast('Không thể tải thông tin kỳ thi', 'error');
        }
    } catch (error) {
        console.error('Error loading ThangDai detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuThemTaiKhoan() {
    const email = document.getElementById('them_EmailTK').value;
    const matKhau = document.getElementById('them_MatKhauTK').value;
    const vaiTro = document.getElementById('them_VaiTro').value;

    if (!email || !matKhau || !vaiTro) {
        toast('Vui lòng nhập các trường bắt buộc', 'error');
        return;
    }

    try {
        const data = {
            email: email,
            matKhau: matKhau,
            vaiTro: vaiTro,
            trangThai: 1
        };

        const result = await apiCall('/admin/taikhoan', 'POST', data);
        
        if (result && result.success) {
            toast('Thêm tài khoản thành công', 'success');
            hideModal('modalThemTaiKhoan');
            loadTaiKhoanList();
        } else {
            toast(result?.data?.message || 'Thêm tài khoản thất bại', 'error');
        }
    } catch (error) {
        console.error('Error adding TaiKhoan:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function openEditTaiKhoan(maTaiKhoan) {
    try {
        const result = await apiCall(`/admin/taikhoan/${maTaiKhoan}`, 'GET');
        
        if (result && result.success && result.data) {
            const tk = result.data;
            document.getElementById('sua_MaTaiKhoan').value = tk.maTaiKhoan;
            document.getElementById('sua_EmailTK').value = tk.email;
            document.getElementById('sua_VaiTro').value = tk.vaiTro;
            document.getElementById('sua_TrangThaiTK').value = tk.trangThai;
            showModal('modalSuaTaiKhoan');
        } else {
            toast('Không thể tải thông tin tài khoản', 'error');
        }
    } catch (error) {
        console.error('Error loading TaiKhoan detail:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function luuSuaTaiKhoan() {
    const maTaiKhoan = document.getElementById('sua_MaTaiKhoan').value;
    const vaiTro = document.getElementById('sua_VaiTro').value;
    const trangThai = document.getElementById('sua_TrangThaiTK').value;

    try {
        const data = {
            maTaiKhoan: parseInt(maTaiKhoan),
            vaiTro: vaiTro,
            trangThai: parseInt(trangThai)
        };

        const result = await apiCall(`/admin/taikhoan/${maTaiKhoan}`, 'PUT', data);
        
        if (result && result.success) {
            toast('Cập nhật tài khoản thành công', 'success');
            hideModal('modalSuaTaiKhoan');
            loadTaiKhoanList();
        } else {
            toast(result?.data?.message || 'Cập nhật tài khoản thất bại', 'error');
        }
    } catch (error) {
        console.error('Error updating TaiKhoan:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}

async function xoaTaiKhoan(maTaiKhoan) {
    try {
        const result = await apiCall(`/admin/taikhoan/${maTaiKhoan}`, 'DELETE');
        
        if (result && result.success) {
            toast('Xóa tài khoản thành công', 'success');
            loadTaiKhoanList();
        } else {
            toast(result?.data?.message || 'Xóa tài khoản thất bại', 'error');
        }
    } catch (error) {
        console.error('Error deleting TaiKhoan:', error);
        toast('Có lỗi xảy ra', 'error');
    }
}
