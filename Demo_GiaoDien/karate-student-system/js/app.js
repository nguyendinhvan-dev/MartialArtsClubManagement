/* =========================
   CHECK LOGIN & INIT APP
========================= */

const currentPage = window.location.pathname.split("/").pop();

// Kiem tra trang thai dang nhap truoc
if (
    !window.API.AuthService.isAuthenticated() &&
    currentPage !== "login.html" &&
    currentPage !== "register.html"
) {
    window.location.href = "../pages/login.html";
}

/* =========================
   KARATE UTE - Thong bao he thong hoat dong
========================= */
console.log("Karate UTE Student System");

/* =========================
   ACTIVE MENU - Tu dong danh dau menu dang hien thi
========================= */
const navLinks = document.querySelectorAll(".nav-link");
navLinks.forEach(link => {
    const linkPage = link.getAttribute("href");
    if (linkPage === currentPage) {
        link.classList.add("active");
    }
});

/* =========================
   BUTTON EFFECT
========================= */
const buttons = document.querySelectorAll("button");
buttons.forEach(button => {
    button.addEventListener("mouseenter", () => {
        button.style.transform = "scale(1.03)";
    });
    button.addEventListener("mouseleave", () => {
        button.style.transform = "scale(1)";
    });
});

/* =========================
   CLOSE BUTTON
========================= */
const closeBtn = document.querySelector(".close-btn a");
if (closeBtn) {
    closeBtn.addEventListener("mouseenter", () => {
        closeBtn.style.transform = "rotate(90deg)";
    });
    closeBtn.addEventListener("mouseleave", () => {
        closeBtn.style.transform = "rotate(0deg)";
    });
}

/* =========================
   LOGOUT
========================= */
function logout() {
    window.API.AuthService.logout();
}

/* =========================
   ASYNC DATA INITIALIZATION
========================= */
async function initApp() {
    if (!window.API.AuthService.isAuthenticated()) return;

    try {
        const profileRes = await window.API.StudentService.getProfile();
        if (profileRes.success) {
            const user = profileRes.data;
            
            // DASHBOARD & PROFILE DATA
            const studentName = document.getElementById("student-name");
            if (studentName) studentName.innerText = user.tenHocVien;

            const profileName = document.getElementById("profile-name");
            if (profileName) profileName.innerText = user.tenHocVien;

            const profileBelt = document.getElementById("profile-belt");
            if (profileBelt) profileBelt.innerText = user.capDaiHienTai;

            const dashboardBelt = document.getElementById("dashboard-belt");
            if (dashboardBelt) dashboardBelt.innerText = user.capDaiHienTai;

            const profileEmail = document.getElementById("profile-email");
            if (profileEmail) profileEmail.innerText = user.email;

            const profilePhone = document.getElementById("profile-phone");
            if (profilePhone) profilePhone.innerText = user.soDienThoai;
        }

        // ATTENDANCE DATA
        const attendanceBox = document.getElementById("attendanceContainer");
        if (attendanceBox || document.getElementById("attendance-count")) {
            const attendanceRes = await window.API.AttendanceService.getAttendanceSummary();
            if (attendanceRes.success) {
                const attendanceData = attendanceRes.data;

                const attendanceCount = document.getElementById("attendance-count");
                if (attendanceCount) attendanceCount.innerText = attendanceData.length;

                if (attendanceBox) {
                    const presentCount = attendanceData.filter(item => item.trangThai === "CoMat" || item.trangThai === "present").length;
                    const lateCount = attendanceData.filter(item => item.trangThai === "DiTre" || item.trangThai === "late").length;
                    const absentCount = attendanceData.filter(item => item.trangThai === "VangMat" || item.trangThai === "absent").length;

                    const presentText = document.getElementById("present-count");
                    if (presentText) presentText.innerText = presentCount;

                    const lateText = document.getElementById("late-count");
                    if (lateText) lateText.innerText = lateCount;

                    const absentText = document.getElementById("absent-count");
                    if (absentText) absentText.innerText = absentCount;

                    const total = presentCount + lateCount + absentCount;
                    let percent = 0;
                    if (total > 0) {
                        percent = Math.round(((presentCount + lateCount) / total) * 100);
                    }

                    const percentText = document.getElementById("attendance-percent");
                    if (percentText) percentText.innerText = percent + "%";

                    const attendanceBar = document.getElementById("attendance-bar");
                    if (attendanceBar) attendanceBar.style.width = percent + "%";

                    if (attendanceData.length === 0) {
                        attendanceBox.innerHTML = `
                        <div class="empty-box">
                            <i class="fa-solid fa-calendar-xmark"></i>
                            <h3>Chưa có dữ liệu điểm danh</h3>
                        </div>`;
                    }
                }
            }
        }

        // EXAM DATA
        const examCount = document.getElementById("exam-count");
        const resultModal = document.getElementById("resultModal");
        let examsData = [];
        if (examCount || resultModal) {
            const examRes = await window.API.ExamService.getExamResults();
            if (examRes.success) {
                examsData = examRes.data;
                if (examCount) examCount.innerText = examsData.length;
            }
        }

        window.examsData = examsData; // save for showResult func

    } catch (error) {
        console.error("Lỗi khi tải dữ liệu từ API:", error);
    }
}

// Gọi hàm khởi tạo
if (currentPage !== "login.html" && currentPage !== "register.html") {
    initApp();
}

/* =========================
   BELT RESULT (Thăng đai)
========================= */
function showResult() {
    document.getElementById("resultModal").classList.add("show-modal");
    const hasResult = window.examsData && window.examsData.length > 0;
    if (hasResult) {
        document.getElementById("passedResult").style.display = "block";
        document.getElementById("noResult").style.display = "none";
    } else {
        document.getElementById("passedResult").style.display = "none";
        document.getElementById("noResult").style.display = "block";
    }
}

function closeResult() {
    const modal = document.getElementById("resultModal");
    if(modal) modal.classList.remove("show-modal");
}

/* =========================
   CHANGE PASSWORD EVENT
========================= */
const changePasswordForm = document.getElementById("changePasswordForm");
if (changePasswordForm) {
    changePasswordForm.addEventListener("submit", async function(e) {
        e.preventDefault();
        const currentPassword = document.getElementById("currentPassword").value;
        const newPassword = document.getElementById("newPassword").value;
        const confirmPassword = document.getElementById("confirmPassword").value;

        if(newPassword !== confirmPassword) {
            alert("Mật khẩu xác nhận không khớp!");
            return;
        }

        try {
            const res = await window.API.SettingService.changePassword(currentPassword, newPassword);
            if(res.success) {
                alert("Đổi mật khẩu thành công!");
                changePasswordForm.reset();
            } else {
                alert(res.message);
            }
        } catch(error) {
            alert("Lỗi kết nối.");
        }
    });
}