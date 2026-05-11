const form =
document.getElementById(
"loginForm"
);

/* =========================
   DEMO ACCOUNT
========================= */

const demoUser = {

    username: "hoang",

    password: "123456",

    name: "Trần Nhật Hoàng",

    belt: "Đai Trắng",

    email: "hoang@gmail.com",

    phone: "0123456789",

    attendance: [],

    exams: [],

    notifications: [

        {

            title: "Chào mừng",

            content:
            "Chào mừng bạn đến với Karate UTE"

        }

    ]

};

/* SAVE DEMO */

localStorage.setItem(
"studentUser",
JSON.stringify(demoUser)
);

/* LOGIN */

form.addEventListener(
"submit",
function(e){

    e.preventDefault();

    const username =
    document.getElementById(
    "username"
    ).value;

    const password =
    document.getElementById(
    "password"
    ).value;

    const user =
    JSON.parse(
    localStorage.getItem(
    "studentUser"
    )
    );

    if(

        username === user.username &&

        password === user.password

    ){

        localStorage.setItem(
        "isLogin",
        "true"
        );

        alert(
        "Đăng nhập thành công"
        );

        window.location.href =
        "dashboard.html";

    }

    else{

        alert(
        "Sai tài khoản hoặc mật khẩu"
        );

    }

});