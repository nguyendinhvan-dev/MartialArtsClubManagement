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
        toast("Vui lòng nhập tên đăng nhập và mật khẩu", 'error');
        return;
    }

    const body = {
        Email: TenDangNhap,
        Password: MatKhau
    };

    try {
        const result = await apiCall('/auth/login', 'POST', body);

        console.log('Login API result:', result);

        if (result && result.success && result.data && result.data.success) {
            const payload = result.data.data;
            const token = payload?.token || payload?.Token;
            const role = payload?.role || payload?.Role;
            const maTaiKhoan = payload?.maTaiKhoan || payload?.MaTaiKhoan;

            if (token) {
                localStorage.setItem('token', token);
                if (role) localStorage.setItem('role', role);
                if (maTaiKhoan) localStorage.setItem('maTaiKhoan', maTaiKhoan);

                const roleLower = role ? role.toLowerCase() : '';

                // Force admin redirect if user is admin
                if (roleLower === 'admin' || roleLower === 'quantri' || roleLower === 'quantrivien') {
                    window.location.href = '/admin/dashboard.html';
                } else if (roleLower === 'huanluyenvien' || roleLower === 'trainer') {
                    window.location.href = '/hlv.html';
                } else {
                    window.location.href = '/pages/dashboard.html';
                }
            } else {
                toast("Đăng nhập thất bại: Không nhận được token.", 'error');
            }
        } else {
            toast(result?.data?.message || "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.", 'error');
        }
    } catch (error) {
        toast("Có lỗi xảy ra: " + error.message, 'error');
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('maTaiKhoan');
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
        const allowedRoles = Array.isArray(requiredRole) ? requiredRole.map(r => r.toLowerCase()) : [requiredRole.toLowerCase()];
        if (!role || !allowedRoles.includes(role.toLowerCase())) {
            toast("Bạn không có quyền truy cập trang này!", 'error');
            setTimeout(() => {
                window.location.href = '/login.html';
            }, 1500);
            return false;
        }
    }

    return true;
}

// Global Toast function for login page if not defined
function toast(msg, type = 'success') {
    if (window.toast && typeof window.toast === 'function' && window.toast !== toast) {
        window.toast(msg, type);
        return;
    }
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
      max-width:320px;
    `;
    el.innerHTML = `<i class="fas ${icons[type] || icons.info}"></i> ${msg}`;
    document.body.appendChild(el);
    setTimeout(() => {
        el.style.opacity = '0';
        el.style.transition = 'opacity .3s';
        setTimeout(() => el.remove(), 300);
    }, 3000);
}
