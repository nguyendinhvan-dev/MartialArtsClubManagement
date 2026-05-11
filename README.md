# Hệ Thống Quản Lý Câu Lạc Bộ Võ Thuật

## 📋 Giới Thiệu

Dự án này là hệ thống quản lý toàn diện cho câu lạc bộ võ thuật, được phát triển bởi nhóm 7 - Lớp 225LTC. 

**⚠️ LƯU Ý: Dự án đang trong giai đoạn phát triển ban đầu**
- Hiện tại: Database schema và giao diện demo (HTML/CSS/JS)
- Sắp tới: Backend API (ASP.NET Core) và tích hợp đầy đủ

Hệ thống hỗ trợ quản lý học viên, huấn luyện viên, khóa học, lớp học, thi thăng đai và các hoạt động liên quan.

## 🏗️ Kiến Trúc Hệ Thống (Dự kiến)

### Giai đoạn hiện tại:
- **Database**: Microsoft SQL Server (đã hoàn thiện schema)
- **Frontend Demo**: HTML5, CSS3, JavaScript, Bootstrap

### Giai đoạn sắp tới:
- **Backend API**: ASP.NET Core (C#)
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core
- **Frontend thực tế**: React/Angular (chưa quyết định)

## 📁 Cấu Trúc Project (Hiện tại)

```
MartialArtsClubManagement/
├── 📄 Nhom7_225LTC_Sharp.sql          # Database schema và sample data (HOÀN THIỆN)
├── 📄 Nhom7_225LTC_Sharp.docx        # Tài liệu dự án
├── 📁 Demo_GiaoDien/                  # Giao diện demo tĩnh
│   └── 📁 Admin_ui/                  # Giao diện admin (HTML/CSS/JS)
│       ├── 📄 dashboard.html          # Trang tổng quan
│       ├── 📄 hocvien.html           # Quản lý học viên
│       ├── 📄 huanluyenvien.html      # Quản lý huấn luyện viên
│       ├── 📄 lophoc.html             # Quản lý lớp học
│       ├── 📄 thangdai.html           # Quản lý thi thăng đai
│       ├── 📄 hocphi.html             # Quản lý học phí
│       ├── 📄 diemdanh.html           # Điểm danh
│       ├── 📄 sukien.html             # Quản lý sự kiện
│       ├── 📄 taikhoan.html           # Quản lý tài khoản
│       ├── 📄 caidat.html             # Cài đặt hệ thống
│       ├── 📄 dangnhap.html           # Trang đăng nhập
│       └── 📄 landingpage.html        # Trang chủ
├── 📁 docs/                           # Tài liệu hướng dẫn
│   ├── 📄 GIT_WORKFLOW.md            # Hướng dẫn Git workflow
│   └── 📄 PROJECT_STRUCTURE.md       # Mô tả cấu trúc chi tiết
├── 📄 README.md                       # File này
├── 📄 CONTRIBUTING.md                 # Hướng dẫn đóng góp
├── 📄 LICENSE                         # Giấy phép
└── 📄 .gitignore                      # File ignore cho Git

## 📁 Cấu Trúc Project (Dự kiến - Giai đoạn Backend)

```
MartialArtsClubManagement/
├── 📁 backend/                        # ASP.NET Core Web API
│   ├── 📁 Controllers/                # API Controllers
│   ├── 📁 Models/                     # Entity Models
│   ├── 📁 DTOs/                       # Data Transfer Objects
│   ├── 📁 Services/                   # Business Logic
│   ├── 📁 Repositories/               # Data Access Layer
│   ├── 📁 Configuration/              # App Settings
│   ├── 📁 Migrations/                 # EF Core Migrations
│   └── 📄 Program.cs                  # Entry point
├── 📁 frontend/                       # React/Angular (chưa quyết định)
├── 📁 tests/                          # Unit & Integration Tests
└── 📁 deployment/                     # Docker, CI/CD configs
```

## 🚀 Bắt Đầu Nhanh

### Giai đoạn hiện tại (Demo)

**Yêu cầu:**
- SQL Server 2019 trở lên
- Web browser (Chrome, Firefox, Edge)

**Cài đặt:**
1. **Clone repository**
   ```bash
   git clone https://github.com/username/MartialArtsClubManagement.git
   cd MartialArtsClubManagement
   ```

2. **Cài đặt database**
   ```bash
   sqlcmd -S your-server-name -i Nhom7_225LTC_Sharp.sql
   ```

3. **Xem demo giao diện**
   - Mở file `Demo_GiaoDien/Admin_ui/landingpage.html`
   - Hoặc chạy local server để xem đầy đủ

### Giai đoạn sắp tới (Backend API)

**Yêu cầu:**
- .NET 6.0 trở lên
- SQL Server 2019 trở lên
- Visual Studio 2022 hoặc Visual Studio Code

**Cài đặt (dự kiến):**
1. **Clone repository**
   ```bash
   git clone https://github.com/username/MartialArtsClubManagement.git
   cd MartialArtsClubManagement
   ```

2. **Chạy backend API**
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```

3. **Truy cập API**
   - API URL: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

## 📊 Tính Năng Chính

### 👤 Quản Lý Người Dùng
- Quản lý tài khoản admin, huấn luyện viên, học viên
- Phân quyền truy cập theo vai trò
- Xác thực và bảo mật

### 🎓 Quản Lý Học Viên
- Thêm, sửa, xóa thông tin học viên
- Quản lý cấp đai hiện tại
- Theo dõi lịch sử thăng đai

### 👨‍🏫 Quản Lý Huấn Luyện Viên
- Quản lý thông tin HLV
- Phân công giảng dạy
- Chuyên môn và lịch làm việc

### 📚 Quản Lý Khóa Học & Lớp Học
- Tạo và quản lý khóa học
- Phân chia lớp theo cấp đai
- Quản lý sĩ số và lịch học

### 🥋 Quản Lý Thi Thăng Đai
- Tạo kỳ thi thăng đai
- Quản lý kết quả thi
- Cập nhật cấp đai cho học viên

### 💰 Quản Lý Học Phí
- Theo dõi học phí khóa học
- Quản lý các gói dạy kèm riêng
- Thanh toán và miễn giảm

### 📅 Điểm Danh & Sự Kiện
- Điểm danh học viên
- Quản lý sự kiện CLB
- Thông báo và đăng ký sự kiện

## 🗄️ Database Schema

Database bao gồm 14 bảng chính:

1. **TaiKhoan** - Thông tin tài khoản đăng nhập
2. **HocVien** - Thông tin chi tiết học viên
3. **HuanLuyenVien** - Thông tin huấn luyện viên
4. **CapDai** - Danh sách cấp đai
5. **KhoaHoc** - Thông tin khóa học
6. **LopHoc** - Thông tin lớp học
7. **DangKyLop** - Đăng ký lớp học
8. **DiemDanh** - Điểm danh học viên
9. **GoiKemRieng** - Các gói dạy kèm
10. **DangKyKem** - Đăng ký dạy kèm
11. **KyThiThangDai** - Kỳ thi thăng đai
12. **KetQuaThi** - Kết quả thi
13. **ThongBao** - Thông báo và sự kiện
14. **DangKySuKien** - Đăng ký sự kiện

## 🔧 Công Nghệ Sử Dụng

### Frontend
- **HTML5/CSS3**: Cấu trúc và styling
- **JavaScript**: Logic client-side
- **Bootstrap 5**: UI framework
- **Font Awesome**: Icons

### Backend
- **ASP.NET Core**: Web framework
- **Entity Framework Core**: ORM
- **ASP.NET Core Identity**: Authentication & Authorization
- **Dapper**: Micro-ORM cho performance

### Database
- **SQL Server**: Hệ quản trị CSDL
- **T-SQL**: Ngôn ngữ truy vấn

## 📱 Giao Diện Demo

Thư mục `Demo_GiaoDien/Admin_ui/` chứa các file HTML demo cho giao diện admin với các tính năng:
- Dashboard với thống kê tổng quan
- Quản lý học viên với tìm kiếm và lọc
- Quản lý huấn luyện viên
- Quản lý lớp học và khóa học
- Quản lý thi thăng đai
- Quản lý học phí và thanh toán
- Hệ thống điểm danh
- Quản lý sự kiện và thông báo

## 🤝 Đóng Góp

Vui lòng đọc file [CONTRIBUTING.md](CONTRIBUTING.md) để biết cách đóng góp vào dự án.

## 📝 Git Workflow

Xem hướng dẫn chi tiết về Git workflow tại [docs/GIT_WORKFLOW.md](docs/GIT_WORKFLOW.md).

## 📄 Giấy Phép

Dự án được phát triển dưới giấy phép MIT - xem file [LICENSE](LICENSE) để biết chi tiết.

## 👨‍💻 Nhóm Phát Triển

**Nhóm 7 - Lớp 225LTC**
- Thành viên nhóm
- Giảng viên hướng dẫn

## 📞 Liên Hệ

- Email: [your-email@example.com]
- GitHub: [https://github.com/username/MartialArtsClubManagement]

## 🔄 Lịch Sử Cập Nhật

- **v1.0.0** (2026-05-11): Phiên bản đầu tiên với đầy đủ tính năng
  - Hoàn thiện database schema
  - Xây dựng giao diện admin demo
  - Tài liệu hóa hệ thống

---

⭐ Nếu dự án này hữu ích, hãy cho chúng tôi một star!
