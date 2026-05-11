# Cấu Trúc Project - CLB Võ Thuất Management System

## 📁 Tổng Quan Cấu Trúc

Dự án được chia thành nhiều giai đoạn phát triển, từ database và giao diện demo hiện tại đến backend API hoàn chỉnh trong tương lai.

## 🗂️ Cấu Trúc Hiện Tại (Giai Đoạn 1)

```
MartialArtsClubManagement/
├── 📄 Nhom7_225LTC_Sharp.sql          # Database schema & sample data
├── 📄 Nhom7_225LTC_Sharp.docx        # Tài liệu đề tài
├── 📁 Demo_GiaoDien/                  # Giao diện demo tĩnh
│   └── 📁 Admin_ui/                  # Giao diện Admin Panel
│       ├── 📄 landingpage.html        # Trang chủ demo
│       ├── 📄 dangnhap.html           # Trang đăng nhập
│       ├── 📄 dashboard.html          # Dashboard tổng quan
│       ├── 📄 hocvien.html            # Quản lý học viên
│       ├── 📄 huanluyenvien.html      # Quản lý huấn luyện viên
│       ├── 📄 lophoc.html             # Quản lý lớp học
│       ├── 📄 thangdai.html           # Quản lý thi thăng đai
│       ├── 📄 hocphi.html             # Quản lý học phí
│       ├── 📄 diemdanh.html           # Điểm danh
│       ├── 📄 sukien.html             # Quản lý sự kiện
│       ├── 📄 taikhoan.html           # Quản lý tài khoản
│       └── 📄 caidat.html             # Cài đặt hệ thống
├── 📁 docs/                           # Tài liệu dự án
│   ├── 📄 GIT_WORKFLOW.md            # Hướng dẫn Git workflow
│   └── 📄 PROJECT_STRUCTURE.md       # File này
├── 📄 README.md                       # Documentation chính
├── 📄 CONTRIBUTING.md                 # Hướng dẫn đóng góp
├── 📄 LICENSE                         # Giấy phép MIT
└── 📄 .gitignore                      # Git ignore file
```

## 🏗️ Cấu Trúc Tương Lai (Giai Đoạn 2 - Backend API)

```
MartialArtsClubManagement/
├── 📁 backend/                        # ASP.NET Core Web API
│   ├── 📄 MartialArtsClubManagement.sln
│   ├── 📄 MartialArtsClubManagement.csproj
│   ├── 📄 Program.cs                  # Entry point
│   ├── 📄 appsettings.json            # Configuration
│   ├── 📄 appsettings.Development.json
│   ├── 📁 Controllers/                # API Controllers
│   │   ├── 📄 AuthController.cs
│   │   ├── 📄 HocVienController.cs
│   │   ├── 📄 HuanLuyenVienController.cs
│   │   ├── 📄 LopHocController.cs
│   │   ├── 📄 KhoaHocController.cs
│   │   ├── 📄 ThangDaiController.cs
│   │   ├── 📄 HocPhiController.cs
│   │   ├── 📄 DiemDanhController.cs
│   │   ├── 📄 SuKienController.cs
│   │   └── 📄 TaiKhoanController.cs
│   ├── 📁 Models/                     # Data Models
│   │   ├── 📁 Entities/               # Database entities
│   │   │   ├── 📄 TaiKhoan.cs
│   │   │   ├── 📄 HocVien.cs
│   │   │   ├── 📄 HuanLuyenVien.cs
│   │   │   ├── 📄 CapDai.cs
│   │   │   ├── 📄 KhoaHoc.cs
│   │   │   ├── 📄 LopHoc.cs
│   │   │   ├── 📄 DangKyLop.cs
│   │   │   ├── 📄 DiemDanh.cs
│   │   │   ├── 📄 GoiKemRieng.cs
│   │   │   ├── 📄 DangKyKem.cs
│   │   │   ├── 📄 KyThiThangDai.cs
│   │   │   ├── 📄 KetQuaThi.cs
│   │   │   ├── 📄 ThongBao.cs
│   │   │   └── 📄 DangKySuKien.cs
│   │   └── 📁 DTOs/                   # Data Transfer Objects
│   │       ├── 📄 Requests/
│   │       │   ├── 📄 LoginRequest.cs
│   │       │   ├── 📄 HocVienCreateRequest.cs
│   │       │   ├── 📄 HocVienUpdateRequest.cs
│   │       │   └── 📄 ...
│   │       └── 📄 Responses/
│   │           ├── 📄 ApiResponse.cs
│   │           ├── 📄 HocVienResponse.cs
│   │           └── 📄 ...
│   ├── 📁 Services/                   # Business Logic
│   │   ├── 📄 Interfaces/
│   │   │   ├── 📄 IHocVienService.cs
│   │   │   ├── 📄 IHuanLuyenVienService.cs
│   │   │   └── 📄 ...
│   │   ├── 📄 HocVienService.cs
│   │   ├── 📄 HuanLuyenVienService.cs
│   │   ├── 📄 AuthService.cs
│   │   ├── 📄 EmailService.cs
│   │   └── 📄 ...
│   ├── 📁 Repositories/               # Data Access Layer
│   │   ├── 📄 Interfaces/
│   │   │   ├── 📄 IHocVienRepository.cs
│   │   │   └── 📄 ...
│   │   ├── 📄 HocVienRepository.cs
│   │   ├── 📄 BaseRepository.cs
│   │   └── 📄 ...
│   ├── 📁 Data/                       # Database context
│   │   ├── 📄 ApplicationDbContext.cs
│   │   └── 📁 Migrations/
│   ├── 📁 Configuration/              # App configuration
│   │   ├── 📄 JwtSettings.cs
│   │   ├── 📄 EmailSettings.cs
│   │   └── 📄 DatabaseSettings.cs
│   ├── 📁 Middleware/                 # Custom middleware
│   │   ├── 📄 ExceptionHandlingMiddleware.cs
│   │   └── 📄 LoggingMiddleware.cs
│   ├── 📁 Extensions/                 # Extension methods
│   │   ├── 📄 ServiceCollectionExtensions.cs
│   │   └── 📄 MiddlewareExtensions.cs
│   ├── 📁 Validators/                 # FluentValidation
│   │   ├── 📄 HocVienValidator.cs
│   │   └── 📄 ...
│   └── 📁 Helpers/                    # Utility classes
│       ├── 📄 PasswordHelper.cs
│       ├── 📄 EmailHelper.cs
│       └── 📄 ...
├── 📁 tests/                          # Test projects
│   ├── 📁 UnitTests/
│   │   ├── 📁 Services/
│   │   ├── 📁 Controllers/
│   │   └── 📁 Repositories/
│   └── 📁 IntegrationTests/
├── 📁 frontend/                       # Frontend application
│   ├── 📁 src/
│   │   ├── 📁 components/
│   │   ├── 📁 pages/
│   │   ├── 📁 services/
│   │   ├── 📁 utils/
│   │   └── 📄 App.js
│   ├── 📁 public/
│   └── 📄 package.json
└── 📁 deployment/                     # Deployment configs
    ├── 📁 docker/
    │   ├── 📄 Dockerfile.api
    │   ├── 📄 Dockerfile.frontend
    │   └── 📄 docker-compose.yml
    ├── 📁 kubernetes/
    │   ├── 📄 api-deployment.yaml
    │   └── 📄 frontend-deployment.yaml
    └── 📁 azure/
        ├── 📄 appservice.json
        └── 📄 pipelines.yml
```

## 📊 Chi Tiết Các Thành Phần

### 🗄️ Database Layer

#### Bảng chính:
1. **TaiKhoan** - Quản lý tài khoản đăng nhập
2. **HocVien** - Thông tin học viên
3. **HuanLuyenVien** - Thông tin huấn luyện viên
4. **CapDai** - Danh sách cấp đai
5. **KhoaHoc** - Khóa học
6. **LopHoc** - Lớp học
7. **DangKyLop** - Đăng ký lớp
8. **DiemDanh** - Điểm danh
9. **GoiKemRieng** - Gói dạy kèm
10. **DangKyKem** - Đăng ký dạy kèm
11. **KyThiThangDai** - Kỳ thi thăng đai
12. **KetQuaThi** - Kết quả thi
13. **ThongBao** - Thông báo sự kiện
14. **DangKySuKien** - Đăng ký sự kiện

#### Views hữu ích:
- **V_HocVienVaCapDai** - Học viên kèm cấp đai
- **V_LopHocChiTiet** - Lớp học chi tiết
- **V_HocVienDuDieuKienThi** - Học viên đủ điều kiện thi
- **V_DangKyKemChoPhanCong** - Đăng ký kèm chờ phân công

### 🎯 API Controllers Structure

#### Authentication:
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
POST /api/auth/logout
```

#### Học viên:
```
GET    /api/hocvien
GET    /api/hocvien/{id}
POST   /api/hocvien
PUT    /api/hocvien/{id}
DELETE /api/hocvien/{id}
GET    /api/hocvien/{id}/lichthi
GET    /api/hocvien/{id}/diemdanh
```

#### Lớp học:
```
GET    /api/lophoc
GET    /api/lophoc/{id}
POST   /api/lophoc
PUT    /api/lophoc/{id}
DELETE /api/lophoc/{id}
GET    /api/lophoc/{id}/hocvien
POST   /api/lophoc/{id}/diemdanh
```

### 🎨 Frontend Structure (Tương lai)

#### Component Structure:
```
src/
├── components/
│   ├── common/
│   │   ├── Header.jsx
│   │   ├── Sidebar.jsx
│   │   └── Footer.jsx
│   ├── hocvien/
│   │   ├── HocVienList.jsx
│   │   ├── HocVienForm.jsx
│   │   └── HocVienDetail.jsx
│   └── ...
├── pages/
│   ├── Dashboard.jsx
│   ├── HocVien.jsx
│   ├── LopHoc.jsx
│   └── ...
├── services/
│   ├── api.js
│   ├── auth.js
│   ├── hocvien.js
│   └── ...
└── utils/
    ├── constants.js
    ├── helpers.js
    └── validators.js
```

## 🔧 Development Workflow

### Environment Setup:
1. **Development**: Local development với hot reload
2. **Staging**: Testing environment
3. **Production**: Live environment

### Build Process:
1. **API Build**: `dotnet build --configuration Release`
2. **Frontend Build**: `npm run build`
3. **Database**: EF Core Migrations

### Testing Strategy:
1. **Unit Tests**: NUnit/xUnit cho business logic
2. **Integration Tests**: Test API endpoints
3. **E2E Tests**: Selenium/Playwright cho UI

## 📋 Quy Tắc Coding

### Backend:
- **C# 10+** features
- **Async/Await** cho I/O operations
- **Dependency Injection**
- **Repository Pattern**
- **Service Layer Architecture**
- **FluentValidation** cho input validation
- **AutoMapper** cho object mapping

### Frontend:
- **React 18+** với hooks
- **Redux Toolkit** cho state management
- **React Router** cho navigation
- **Axios** cho API calls
- **Tailwind CSS** cho styling

### Database:
- **EF Core** với Code First
- **Fluent API** cho configurations
- **Migrations** cho schema changes

## 🚀 Deployment Architecture

### Local Development:
```
Developer Machine
├── SQL Server LocalDB
├── .NET API (localhost:5001)
└── React Dev Server (localhost:3000)
```

### Production:
```
Azure App Service
├── API Container
├── Frontend Container
└── Azure SQL Database
```

## 📝 Documentation Structure

### Code Documentation:
- **XML Comments** cho public APIs
- **README** cho mỗi module
- **Architecture Decision Records (ADRs)**

### API Documentation:
- **Swagger/OpenAPI** tự động sinh
- **Postman Collection** cho testing
- **API Blueprint** cho documentation

---

**Lưu ý**: Cấu trúc này sẽ được cập nhật khi dự án phát triển qua các giai đoạn tiếp theo.
