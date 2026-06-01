<div align="center">

# 🥋 HỆ THỐNG QUẢN LÝ CÂU LẠC BỘ VÕ THUẬT

![Martial Arts Club Management](https://img.shields.io/badge/Martial%20Arts-Club%20Management-red?style=for-the-badge&logo=martial-arts&logoSize=auto&labelColor=darkred)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-7.0-512BD4?style=for-the-badge&logo=dotnet&logoSize=auto&labelColor=darkblue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoSize=auto&labelColor=darkred)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoSize=auto&labelColor=purple)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge&logoSize=auto&labelColor=darkgreen)

**Một hệ thống quản lý toàn diện cho câu lạc bộ võ thuật**

[Features](#-chức-năng-chính) • [Tech Stack](#-công-nghệ-sử-dụng) • [Installation](#-cài-đặt) • [API Documentation](#-api-documentation) • [Team](#-team-phát-triển) • [Demo](#-demo-giao-diện)

</div>

---

# 🎨 DEMO GIAO DIỆN

<div align="center">

| Module | Demo Link |
|--------|-----------|
| 👨‍💼 Admin Panel | [Xem Demo](https://nguyendinhvan-dev.github.io/DemoGiaodienAdmin/) |
| 👨‍🏫 Huấn Luyện Viên | [Xem Demo](https://nguyendinhvan-dev.github.io/DemoGiaodienHuanLuyenVien/) |
| 👨‍🎓 Học Viên | [Xem Demo](https://nguyendinhvan-dev.github.io/DemoGiaodienHocVien/) |

</div>

---

# 📋 GIỚI THIỆU

Hệ thống Quản Lý Câu Lạc Bộ Võ Thuật là một giải pháp quản lý toàn diện được phát triển bởi **Nhóm 7 - Lớp 225LTC**. Hệ thống hỗ trợ quản lý học viên, huấn luyện viên, khóa học, lớp học, thi thăng đai và các hoạt động liên quan đến vận hành câu lạc bộ võ thuật.

### ✨ Điểm Nổi Bật

- 🔐 **Authentication & Authorization**: JWT-based authentication với role-based access control
- 📊 **Dashboard Thông Minh**: Thống kê và báo cáo trực quan
- 🎯 **Multi-Role System**: Hỗ trợ 3 vai trò: Admin, Huấn Luyện Viên, Học Viên
- 📱 **Responsive Design**: Giao diện tương thích mọi thiết bị
- 🚀 **RESTful API**: API chuẩn RESTful với Swagger documentation
- 💾 **Database Optimization**: 14 bảng với quan hệ hoàn chỉnh, tối ưu hóa query

---

# 🎯 CHỨC NĂNG CHÍNH

## 👨‍💼 ADMIN MODULE

<div align="center">

| Chức năng | Mô tả |
|-----------|-------|
| 📊 Dashboard | Thống kê tổng quan, biểu đồ, báo cáo |
| 👥 Quản lý Học Viên | Thêm, sửa, xóa, xem chi tiết học viên |
| 🏫 Quản lý Lớp Học | Quản lý lớp học, khóa học, lịch học |
| 👨‍🏫 Quản lý HLV | Quản lý huấn luyện viên, phân công |
| ✅ Điểm Danh | Điểm danh học viên (đơn lẻ & hàng loạt) |
| 💰 Học Phí | Quản lý học phí, xác nhận thanh toán |
| 🥋 Thi Thăng Đai | Quản lý kỳ thi, kết quả thi thăng đai |
| 📢 Thông Báo | Quản lý thông báo, sự kiện |
| ⚙️ Cài Đặt | Cấu hình hệ thống, quản lý tài khoản |

</div>

## 👨‍🏫 HUẤN LUYỆN VIÊN MODULE

<div align="center">

| Chức năng | Mô tả |
|-----------|-------|
| 📋 Lịch Làm Việc | Xem lịch dạy, lịch điểm danh |
| ✅ Điểm Danh | Điểm danh học viên trong lớp |
| 📝 Đánh Giá | Đánh giá tiến độ học viên |
| 🥋 Quản Lý Thi | Tổ chức và chấm thi thăng đai |
| 👨‍🏫 Dạy Kèm Riêng | Quản lý lịch dạy kèm riêng |

</div>

## 👨‍🎓 HỌC VIÊN MODULE

<div align="center">

| Chức năng | Mô tả |
|-----------|-------|
| 👤 Hồ Sơ Cá Nhân | Xem và cập nhật thông tin cá nhân |
| 📅 Đăng Ký Lớp | Xem và đăng ký lớp học |
| 📜 Lịch Tập | Xem lịch tập, lịch thi |
| 🥋 Thăng Đai | Xem lịch thi, kết quả thi |
| 💸 Học Phí | Xem học phí, thanh toán |
| 📢 Thông Báo | Nhận thông báo từ CLB |

</div>

---

# 🛠️ CÔNG NGHỆ SỬ DỤNG

## 🔧 BACKEND

<div align="center">

![C#](https://img.shields.io/badge/C%23-10-239120?style=for-the-badge&logo=c-sharp&logoColor=white&labelColor=darkgreen)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-7.0-512BD4?style=for-the-badge&logo=dotnet&labelColor=darkblue)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=for-the-badge&logo=entity-framework&labelColor=darkblue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server&labelColor=darkred)
![JWT](https://img.shields.io/badge/JWT-Authentication-FF6F00?style=for-the-badge&logo=json-web-tokens&labelColor=orange)

</div>

- **Framework**: ASP.NET Core 7.0 Web API
- **Language**: C# 10
- **ORM**: Entity Framework Core 7.0
- **Database**: Microsoft SQL Server 2019
- **Authentication**: JWT (JSON Web Token)
- **Password Hashing**: BCrypt
- **API Documentation**: Swagger/OpenAPI

## 🎨 FRONTEND

<div align="center">

![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white&labelColor=darkorange)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white&labelColor=darkblue)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black&labelColor=yellow)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&labelColor=purple)
![Font Awesome](https://img.shields.io/badge/Font%20Awesome-6.4-339AF0?style=for-the-badge&logo=font-awesome&labelColor=darkblue)

</div>

- **Structure**: HTML5
- **Styling**: CSS3 + Bootstrap 5.3
- **Logic**: JavaScript ES6+
- **UI Framework**: Bootstrap 5
- **Icons**: Font Awesome 6
- **HTTP Client**: Fetch API

## 💾 DATABASE

<div align="center">

![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server&labelColor=darkred)
![Tables](https://img.shields.io/badge/Tables-14-blue?style=for-the-badge&labelColor=darkblue)
![Relationships](https://img.shields.io/badge/Relationships-Complete-green?style=for-the-badge&labelColor=darkgreen)

</div>

- **14 bảng chính** với quan hệ hoàn chỉnh
- **Views** tối ưu hóa query
- **Stored Procedures** (nếu cần)
- **Constraints** và **Indexes** tối ưu

---

# 📐 CẤU TRÚC DATABASE

## 📊 BẢNG CHÍNH

<div align="center">

| Bảng | Mô tả |
|------|-------|
| `TaiKhoan` | Tài khoản đăng nhập, phân quyền |
| `HocVien` | Thông tin học viên |
| `HuanLuyenVien` | Thông tin huấn luyện viên |
| `CapDai` | Danh sách cấp đai (7 cấp) |
| `KhoaHoc` | Khóa học |
| `LopHoc` | Lớp học |
| `DangKyLop` | Đăng ký lớp học |
| `DiemDanh` | Điểm danh |
| `GoiKemRieng` | Gói dạy kèm riêng |
| `DangKyKem` | Đăng ký dạy kèm |
| `KyThiThangDai` | Kỳ thi thăng đai |
| `KetQuaThi` | Kết quả thi |
| `ThongBao` | Thông báo sự kiện |
| `DangKySuKien` | Đăng ký sự kiện |

</div>

## 👁️ VIEWS HỮU ÍCH

- `V_HocVienVaCapDai` - Học viên kèm cấp đai
- `V_LopHocChiTiet` - Lớp học chi tiết
- `V_HocVienDuDieuKienThi` - Học viên đủ điều kiện thi
- `V_DangKyKemChoPhanCong` - Đăng ký kèm chờ phân công

---

# 🚀 CÀI ĐẶT

## ⚙️ YÊU CẦU HỆ THỐNG

- **.NET 7.0 SDK** hoặc cao hơn
- **SQL Server 2019** hoặc cao hơn
- **Node.js** (cho frontend development - tùy chọn)
- **Git** (để clone repository)

## 📥 BƯỚC 1: CLONE REPOSITORY

```bash
git clone https://github.com/nguyendinhvan-dev/MartialArtsClubManagement.git
cd MartialArtsClubManagement
```

## 🗄️ BƯỚC 2: CÀI ĐẶT DATABASE

```bash
# Mở SQL Server Management Studio (SSMS)
# Chạy file SQL: Nhom7_225LTC_Sharp.sql
# Database sẽ được tạo tự động với dữ liệu mẫu
```

Hoặc dùng SQLCMD:

```bash
sqlcmd -S localhost -U sa -P YourPassword -i Nhom7_225LTC_Sharp.sql
```

## ⚙️ BƯỚC 3: CẤU HÌNH CONNECTION STRING

Mở file `backend/MartialArtsClubManagement/MartialArtsClubManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=QuanLyCLBVoThuat;Integrated Security=True;Trust Server Certificate=True"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "MartialArtsClub",
    "Audience": "MartialArtsClubClients",
    "ExpiresInMinutes": 60
  }
}
```

## 🏃 BƯỚC 4: CHẠY BACKEND API

```bash
cd backend/MartialArtsClubManagement/MartialArtsClubManagement.API
dotnet restore
dotnet build
dotnet run
```

API sẽ chạy tại: `http://localhost:5000`

Swagger UI: `http://localhost:5000/swagger`

## 🌐 BƯỚC 5: TRUY CẬP FRONTEND

Mở trình duyệt và truy cập:

- **Admin Panel**: `http://localhost:5000/admin/dashboard.html`
- **Student Portal**: `http://localhost:5000/index.html`
- **Trainer Portal**: `http://localhost:5000/hlv.html`

---

# 📚 API DOCUMENTATION

## 🔐 AUTHENTICATION

```bash
# Login
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@clb.vn",
  "password": "password"
}

# Response
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "maTaiKhoan": 1,
    "hoTen": "Admin",
    "email": "admin@clb.vn",
    "vaiTro": "QuanTriVien"
  }
}
```

## 📡 ADMIN API ENDPOINTS

### 👥 HỌC VIÊN

```bash
GET    /api/admin/hocvien              # Lấy danh sách
GET    /api/admin/hocvien/{id}         # Lấy chi tiết
POST   /api/admin/hocvien              # Tạo mới
PUT    /api/admin/hocvien/{id}         # Cập nhật
DELETE /api/admin/hocvien/{id}         # Xóa
```

### 🏫 LỚP HỌC

```bash
GET    /api/admin/lophoc               # Lấy danh sách
GET    /api/admin/lophoc/{id}          # Lấy chi tiết
POST   /api/admin/lophoc               # Tạo mới
PUT    /api/admin/lophoc/{id}          # Cập nhật
DELETE /api/admin/lophoc/{id}          # Xóa
```

### ✅ ĐIỂM DANH

```bash
GET    /api/admin/diemdanh             # Lấy danh sách
POST   /api/admin/diemdanh             # Điểm danh đơn
POST   /api/admin/diemdanh/bulk        # Điểm danh hàng loạt
PUT    /api/admin/diemdanh/{id}        # Cập nhật
DELETE /api/admin/diemdanh/{id}        # Xóa
```

## 🎓 STUDENT API ENDPOINTS

```bash
GET    /api/hocvien/portal/profile     # Hồ sơ cá nhân
GET    /api/hocvien/portal/classes     # Lớp đã đăng ký
POST   /api/hocvien/portal/enroll      # Đăng ký lớp
GET    /api/hocvien/portal/exams       # Lịch thi
GET    /api/hocvien/portal/tuition     # Học phí
```

## 📖 SWAGGER DOCUMENTATION

Truy cập Swagger UI tại: `http://localhost:5000/swagger`

---

# 📁 CẤU TRÚC PROJECT

```
MartialArtsClubManagement/
├── 📄 Nhom7_225LTC_Sharp.sql          # Database schema & seed data
├── 📁 backend/                        # ASP.NET Core Web API
│   └── MartialArtsClubManagement.API/
│       ├── Controllers/               # 21 API Controllers
│       │   ├── AdminHocVienController.cs
│       │   ├── AdminLopHocController.cs
│       │   ├── AdminDashboardController.cs
│       │   ├── AuthController.cs
│       │   └── ...
│       ├── Models/
│       │   ├── Entities/              # 19 Database Entities
│       │   │   ├── HocVien.cs
│       │   │   ├── LopHoc.cs
│       │   │   ├── TaiKhoan.cs
│       │   │   └── ...
│       │   └── DTOs/                  # 17 Data Transfer Objects
│       │       ├── AdminHocVienDto.cs
│       │       └── ...
│       ├── Services/                  # Business Logic
│       ├── Program.cs                 # Entry point
│       ├── appsettings.json           # Configuration
│       └── wwwroot/                   # Static files
│           ├── admin/                 # Admin UI (9 pages)
│           ├── pages/                 # Student Portal (12 pages)
│           ├── js/                    # JavaScript files
│           └── css/                   # CSS files
├── 📁 Demo_GiaoDien/                  # Demo UI
│   └── Admin_ui/                     # Static demo pages
├── 📁 docs/                           # Documentation
│   ├── PROJECT_STRUCTURE.md
│   ├── GIT_WORKFLOW.md
│   └── PROJECT_PLAN.md
└── 📄 README.md                       # This file
```

---

# 👥 TEAM PHÁT TRIỂN

<div align="center">

### Nhóm 7 - Lớp 225LTC

| Thành Viên | Vai Trò | GitHub | Profile |
|------------|---------|--------|---------|
| Nguyễn Đình Văn | Team Lead | [@nguyendinhvan-dev](https://github.com/nguyendinhvan-dev) | [Xem Profile](https://github.com/nguyendinhvan-dev) |
| Trần Nhật Hoàng | Backend Developer | [@nhathoang206](https://github.com/nhathoang206) | - |
| Nguyễn Đăng Thắng | Frontend Developer | [@nguyenthang24032006-bit](https://github.com/nguyenthang24032006-bit) | - |

</div>

---

# 🔗 LIÊN KẾT QUAN TRỌNG

<div align="center">

### 📁 GitHub Repository
[![GitHub Repo stars](https://img.shields.io/github/stars/nguyendinhvan-dev/MartialArtsClubManagement?style=social)](https://github.com/nguyendinhvan-dev/MartialArtsClubManagement)
[![GitHub forks](https://img.shields.io/github/forks/nguyendinhvan-dev/MartialArtsClubManagement?style=social)](https://github.com/nguyendinhvan-dev/MartialArtsClubManagement/fork)

**Link**: https://github.com/nguyendinhvan-dev/MartialArtsClubManagement

---

### 📋 Trello Board
**Link**: https://trello.com/invite/b/6a01473f30e300725dae401e/ATTIdcdd577c2cc1573a9ab154c18a5a32ee884E492A/clb-vo-thuật-nhom-7

---

### 📄 Google Docs
**Link**: https://docs.google.com/document/d/1okyj081sd7XhCre5gg9EUSa7EIM6R31uq8g6e1nnseY/edit?tab=t.0

</div>

---

# 📊 THỐNG KÊ PROJECT

<div align="center">

![Lines of Code](https://img.shields.io/badge/Code-10%2C000%2B-brightgreen?style=for-the-badge&labelColor=darkgreen)
![Controllers](https://img.shields.io/badge/Controllers-21-blue?style=for-the-badge&labelColor=darkblue)
![DTOs](https://img.shields.io/badge/DTOs-17-orange?style=for-the-badge&labelColor=darkorange)
![Entities](https://img.shields.io/badge/Entities-19-purple?style=for-the-badge&labelColor=purple)
![Database Tables](https://img.shields.io/badge/Tables-14-red?style=for-the-badge&labelColor=darkred)
![Frontend Pages](https://img.shields.io/badge/Pages-21-cyan?style=for-the-badge&labelColor=darkblue)

</div>

---

# 🤝 ĐÓNG GÓP

Chúng tôi rất hoan nghênh mọi đóng góp! Nếu bạn muốn đóng góp vào dự án này:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

Xem [CONTRIBUTING.md](CONTRIBUTING.md) để biết thêm chi tiết.

---

# 📝 LICENSE

Dự án này được cấp phép theo Giấy phép MIT - xem file [LICENSE](LICENSE) để biết chi tiết.

---

# 📧 LIÊN HỆ

<div align="center">

**Nhóm 7 - Lớp 225LTC**

[![GitHub](https://img.shields.io/badge/GitHub-nguyendinhvan--dev-black?style=flat-square&logo=github)](https://github.com/nguyendinhvan-dev)
[![Email](https://img.shields.io/badge/Email-Contact%20Us-red?style=flat-square&logo=gmail)](mailto:contact@clb.vn)

---

**Made with ❤️ by Nhóm 7**

</div>
