document.addEventListener('DOMContentLoaded', () => {
    const btnLogin = document.getElementById('btnLogin');
    if (btnLogin) {
        btnLogin.addEventListener('click', handleLogin);
    }
});

async function handleLogin() {
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');
    
    if (!usernameInput || !passwordInput) return;

    const TenDangNhap = usernameInput.value.trim();
    const MatKhau = passwordInput.value.trim();

    if (!TenDangNhap || !MatKhau) {
        alert("Vui lòng nhập tên đăng nhập và mật khẩu");
        return;
    }

    const body = {
        Email: TenDangNhap,
        Password: MatKhau
    };

    try {
        const result = await apiCall('/auth/login', 'POST', body);
        
        // Debug: Log full response to console for troubleshooting
        console.log('Login API result:', result);
        
        if (result && result.success) {
            // Extract payload correctly (ApiResponse wrapper)
            const payload = result.data?.Data || result.data?.data || result.data;
            const token = payload?.Token || payload?.token;
            const role = payload?.Role || payload?.role;
            const email = payload?.Email || payload?.email;

            if (token) {
                localStorage.setItem('token', token);
                if (role) localStorage.setItem('role', role);
                if (email) localStorage.setItem('email', email);

                // Force admin redirect if user is admin (by role or email)
                if (role && (role === 'Admin' || role === 'QuanTri' || role === 'QuanTriVien')) {
                    window.location.href = '/admin/dashboard.html';
                } else if (email && email.toLowerCase() === 'admin@clb.vn') {
                    window.location.href = '/admin/dashboard.html';
                } else if (role && role === 'HuanLuyenVien') {
                    window.location.href = '/huanluyenvien/dashboard.html';
                } else {
                    window.location.href = '/hocvien/dashboard.html';
                }
            } else {
                alert("Đăng nhập thất bại: Không nhận được token.");
            }
        } else {
            alert(result?.data?.message || "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.");
        }
    } catch (error) {
        alert("Có lỗi xảy ra: " + error.message);
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    window.location.href = '/login.html';
}

function checkAuth(requiredRole = null) {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    if (!token) {
        window.location.href = '/login.html';
        return false;
    }

    if (requiredRole) {
        // requiredRole can be a string or an array of strings
        const allowedRoles = Array.isArray(requiredRole) ? requiredRole : [requiredRole];
        if (!allowedRoles.includes(role)) {
            alert("Bạn không có quyền truy cập trang này!");
            window.location.href = '/login.html';
            return false;
        }
    }

    return true;
}
