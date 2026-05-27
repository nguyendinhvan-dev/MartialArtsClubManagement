const form = document.getElementById("loginForm");

form.addEventListener("submit", async function(e){
    e.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const btn = form.querySelector("button[type='submit']");
    const oldText = btn.innerText;
    btn.innerText = "Đang đăng nhập...";
    btn.disabled = true;

    try {
        const response = await window.API.AuthService.login(username, password);

        if(response.success){
            localStorage.setItem("jwtToken", response.data.token);
            alert("Đăng nhập thành công");
            window.location.href = "dashboard.html";
        } else {
            alert(response.message || "Sai tài khoản hoặc mật khẩu");
        }
    } catch (error) {
        alert("Lỗi kết nối đến máy chủ: " + error.message);
    } finally {
        btn.innerText = oldText;
        btn.disabled = false;
    }
});