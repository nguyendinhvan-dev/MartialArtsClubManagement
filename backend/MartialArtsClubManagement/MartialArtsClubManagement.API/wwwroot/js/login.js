const form = document.getElementById("loginForm");

form.addEventListener("submit", async function(e) {
    e.preventDefault();

    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        alert("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
        return;
    }

    try {
        const response = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Username: username, Password: password })
        });

        const result = await response.json();

        if (response.ok && result.success) {
            // Save token and role to localStorage
            localStorage.setItem("token", result.data.token);
            localStorage.setItem("role", result.data.role);
            localStorage.setItem("userId", result.data.userId);
            localStorage.setItem("isLogin", "true");

            alert("Đăng nhập thành công!");

            // Route based on role
            const role = result.data.role.toLowerCase();
            if (role === 'admin') {
                window.location.href = '/admin/dashboard.html';
            } else if (role === 'huanluyenvien') {
                // Assuming trainer dashboard is huanluyenvien.html in admin folder, or their own area
                window.location.href = '/admin/huanluyenvien.html'; // Adjust this if there's a specific trainer UI
            } else {
                // Hoc Vien (Student)
                window.location.href = '/pages/dashboard.html';
            }
        } else {
            alert(result.message || "Đăng nhập thất bại. Sai tài khoản hoặc mật khẩu.");
        }
    } catch (error) {
        console.error("Login error:", error);
        alert("Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.");
    }
});