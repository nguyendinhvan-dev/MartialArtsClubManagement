// Authentication Functions

// Check if user is authenticated
function isAuthenticated() {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');
    return token && role;
}

// Check if user has specific role
function hasRole(expectedRole) {
    const role = localStorage.getItem('role');
    return role === expectedRole;
}

// Redirect based on role
function redirectBasedOnRole() {
    const role = localStorage.getItem('role');
    
    if (!role) {
        window.location.href = '../login.html';
        return;
    }

    switch (role) {
        case 'Admin':
            window.location.href = '../admin/dashboard.html';
            break;
        case 'HocVien':
            window.location.href = '../hocvien/dashboard.html';
            break;
        case 'HuanLuyenVien':
            window.location.href = '../huanluyenvien/dashboard.html';
            break;
        default:
            window.location.href = '../login.html';
    }
}

// Protect page - redirect if not authenticated or wrong role
function protectPage(expectedRole) {
    if (!isAuthenticated()) {
        window.location.href = '../login.html';
        return;
    }

    if (expectedRole && !hasRole(expectedRole)) {
        redirectBasedOnRole();
    }
}

// Handle login
async function handleLogin(event) {
    event.preventDefault();
    
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    
    try {
        const response = await login(username, password);
        
        if (response.success) {
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('role', response.data.role);
            localStorage.setItem('username', response.data.username);
            
            redirectBasedOnRole();
        } else {
            alert('Đăng nhập thất bại: ' + response.message);
        }
    } catch (error) {
        alert('Lỗi đăng nhập: ' + error.message);
    }
}

// Handle logout
function handleLogout() {
    if (confirm('Bạn có chắc muốn đăng xuất?')) {
        logout();
    }
}

// Display user info in UI
function displayUserInfo() {
    const username = getUsername();
    const role = getRole();
    
    const usernameElements = document.querySelectorAll('.display-username');
    const roleElements = document.querySelectorAll('.display-role');
    
    usernameElements.forEach(el => {
        el.textContent = username || 'Không xác định';
    });
    
    roleElements.forEach(el => {
        const roleNames = {
            'Admin': 'Quản trị viên',
            'HocVien': 'Học viên',
            'HuanLuyenVien': 'Huấn luyện viên'
        };
        el.textContent = roleNames[role] || role || 'Không xác định';
    });
}

// Initialize authentication on page load
function initAuth(expectedRole) {
    protectPage(expectedRole);
    displayUserInfo();
}
