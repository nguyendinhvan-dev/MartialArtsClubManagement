/* =========================
   CHECK LOGIN - Kiem tra trang thai dang nhap cua nguoi dung
========================= */

const user =
JSON.parse(
localStorage.getItem(
"studentUser"
)
);

const currentPage =
window.location.pathname
.split("/")
.pop();

/* CHƯA LOGIN - Neu chua dang nhap thi chuyen huong ve trang login */

if(

    !user &&

    currentPage !== "login.html" &&

    currentPage !== "register.html"

){

    window.location.href =
    "../pages/login.html";

}

/* =========================
   KARATE UTE - Thong bao he thong hoat dong
========================= */

console.log(
"Karate UTE Student System"
);

/* =========================
   ACTIVE MENU - Tu dong danh dau menu dang hien thi
========================= */

const navLinks =
document.querySelectorAll(
".nav-link"
);

navLinks.forEach(link => {

    const linkPage =
    link.getAttribute("href");

    if(linkPage === currentPage){

        link.classList.add(
        "active"
        );

    }

});

/* =========================
   BUTTON EFFECT - Hieu ung phong to khi di chuot qua nut
========================= */

const buttons =
document.querySelectorAll(
"button"
);

buttons.forEach(button => {

    button.addEventListener(
    "mouseenter",
    () => {

        button.style.transform =
        "scale(1.03)";

    });

    button.addEventListener(
    "mouseleave",
    () => {

        button.style.transform =
        "scale(1)";

    });

});

/* =========================
   CLOSE BUTTON - Hieu ung xoay cho nut dong
========================= */

const closeBtn =
document.querySelector(
".close-btn a"
);

if(closeBtn){

    closeBtn.addEventListener(
    "mouseenter",
    () => {

        closeBtn.style.transform =
        "rotate(90deg)";

    });

    closeBtn.addEventListener(
    "mouseleave",
    () => {

        closeBtn.style.transform =
        "rotate(0deg)";

    });

}

/* =========================
   DASHBOARD NAME - Hien thi ten hoc vien tai trang chu
========================= */

const studentName =
document.getElementById(
"student-name"
);

if(studentName && user){

    studentName.innerText =
    user.name;

}

/* =========================
   PROFILE - Cap nhat thong tin ca nhan vao giao dien
========================= */

const profileName =
document.getElementById(
"profile-name"
);

if(profileName && user){

    profileName.innerText =
    user.name;

}

const profileBelt =
document.getElementById(
"profile-belt"
);

if(profileBelt && user){

    profileBelt.innerText =
    user.belt;

}

const profileEmail =
document.getElementById(
"profile-email"
);

if(profileEmail && user){

    profileEmail.innerText =
    user.email;

}

const profilePhone =
document.getElementById(
"profile-phone"
);

if(profilePhone && user){

    profilePhone.innerText =
    user.phone;

}

/* =========================
   DASHBOARD DATA - Hien thi so luong diem danh va thi cu
========================= */

const attendanceCount =
document.getElementById(
"attendance-count"
);

if(attendanceCount && user){

    attendanceCount.innerText =
    user.attendance.length;

}

const examCount =
document.getElementById(
"exam-count"
);

if(examCount && user){

    examCount.innerText =
    user.exams.length;

}

/* =========================
   DASHBOARD BELT - Hien thi cap bac dai tai dashboard
========================= */

const dashboardBelt =
document.getElementById(
"dashboard-belt"
);

if(dashboardBelt && user){

    dashboardBelt.innerText =
    user.belt;

}

/* =========================
   NOTIFICATION TOGGLE - Bat tat hien thi chi tiet thong bao
========================= */

const notificationCards =
document.querySelectorAll(
".notification-card"
);

notificationCards.forEach(card => {

    card.addEventListener(
    "click",
    () => {

        card.classList.toggle(
        "active"
        );

    });

});

/* =========================
   BELT RESULT - Hien thi modal ket qua thi dai
========================= */

function showResult(){

    document.getElementById(
"resultModal"
).classList.add(
"show-modal"
);

    const hasResult =
    user.exams.length > 0;

    if(hasResult){

        document.getElementById(
        "passedResult"
        ).style.display =
        "block";

        document.getElementById(
        "noResult"
        ).style.display =
        "none";

    }

    else{

        document.getElementById(
        "passedResult"
        ).style.display =
        "none";

        document.getElementById(
        "noResult"
        ).style.display =
        "block";

    }

}

function closeResult(){

    const modal =
    document.getElementById(
    "resultModal"
    );

    modal.classList.remove(
    "show-modal"
    );

}

/* =========================
   ATTENDANCE - Xu ly du lieu va hien thi thong ke diem danh
========================= */

const attendanceData =
user ? user.attendance : [];

const attendanceBox =
document.getElementById(
"attendanceContainer"
);

if(attendanceBox){

    const presentCount =
    attendanceData.filter(
    item => item.status ===
    "present"
    ).length;

    const lateCount =
    attendanceData.filter(
    item => item.status ===
    "late"
    ).length;

    const absentCount =
    attendanceData.filter(
    item => item.status ===
    "absent"
    ).length;

    const presentText =
    document.getElementById(
    "present-count"
    );

    if(presentText){

        presentText.innerText =
        presentCount;

    }

    const lateText =
    document.getElementById(
    "late-count"
    );

    if(lateText){

        lateText.innerText =
        lateCount;

    }

    const absentText =
    document.getElementById(
    "absent-count"
    );

    if(absentText){

        absentText.innerText =
        absentCount;

    }

    const total =
    presentCount +
    lateCount +
    absentCount;

    let percent = 0;

    if(total > 0){

        percent =
        Math.round(

        ((presentCount + lateCount)
        / total) * 100

        );

    }

    const percentText =
    document.getElementById(
    "attendance-percent"
    );

    if(percentText){

        percentText.innerText =
        percent + "%";

    }

    const attendanceBar =
    document.getElementById(
    "attendance-bar"
    );

    if(attendanceBar){

        attendanceBar.style.width =
        percent + "%";

    }

    /* EMPTY - Hien thi thong bao neu chua co du lieu diem danh */

    if(attendanceData.length === 0){

        attendanceBox.innerHTML = `

        <div class="empty-box">

            <i class="fa-solid fa-calendar-xmark"></i>

            <h3>
                Chưa có dữ liệu điểm danh
            </h3>

            <p>
                Học viên mới chưa tham gia buổi tập nào.
            </p>

        </div>

        `;

    }

}

/* =========================
   LOGOUT - Dang xuat khoi he thong
========================= */

function logout(){

    localStorage.removeItem(
    "studentUser"
    );

    window.location.href =
    "../pages/login.html";

}