# Hướng dẫn Triển khai Frontend - Quản lý CLB Võ Thuật

## 📋 Mục lục
1. [Tổng quan](#tổng-quan)
2. [Cấu trúc thư mục](#cấu-trúc-thư-mục)
3. [Luồng hoạt động Web](#luồng-hoạt-động-web)
4. [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
5. [Cài đặt và thiết lập](#cài-đặt-và-thiết-lập)
6. [Tích hợp Frontend với Backend API](#tích-hợp-frontend-với-backend-api)
7. [Hướng dẫn phát triển](#hướng-dẫn-phát-triển)
8. [Troubleshooting](#troubleshooting)

---

## 📌 Tổng quan

Dự án sử dụng kiến trúc **Client-Server** với:
- **Backend:** ASP.NET Core Web API (C# .NET 10)
- **Frontend:** HTML/CSS/JavaScript + Bootstrap 5
- **Database:** SQL Server (Entity Framework Core)
- **Authentication:** JWT Token

- **Authentication:** JWT Token

---

## 🌐 Luồng hoạt động Web

### Quy trình truy cập hệ thống:

```
1. Landing Page (index.html)
   ↓
   [User bấm nút "Dashboard"]
   ↓
2. Login Page (login.html)
   - Sử dụng form đăng nhập từ Demo_GiaoDien/Admin_ui/login.html
   - Form chung cho tất cả actors (Admin, Học viên, HLV)
   ↓
   [User nhập username/password → Submit]
   ↓
3. Backend Authentication
   - Gọi POST /api/auth/login
   - Backend verify credentials
   - Backend trả về JWT token + Role
   ↓
4. Frontend lưu token
   - Lưu JWT token vào localStorage
   - Lưu Role vào localStorage
   ↓
5. Role-based Routing
   - Nếu Role = "Admin" → Chuyển đến admin/dashboard.html
   - Nếu Role = "HocVien" → Chuyển đến hocvien/dashboard.html
   - Nếu Role = "HuanLuyenVien" → Chuyển đến huanluyenvien/dashboard.html
   ↓
6. Dashboard theo Actor
   - Admin: Quản lý toàn bộ hệ thống
   - Học viên: Xem thông tin cá nhân, điểm danh, thi, học phí
   - HLV: Xem lịch dạy, điểm danh học viên, thi
```

### Chi tiết từng bước:

**Bước 1 - Landing Page:**
- File: `frontend/index.html`
- Nội dung: Giới thiệu CLB Võ Thuật, thông tin chung
- Nút "Dashboard" → Chuyển đến `login.html`
- Giao diện: Sử dụng từ Demo_GiaoDien (nếu có)

**Bước 2 - Login Page:**
- File: `frontend/login.html`
- Form đăng nhập: Copy từ `Demo_GiaoDien/Admin_ui/login.html`
- Input: Username, Password
- JavaScript: Gọi API `/api/auth/login`
- Nhận: JWT token + Role từ backend

**Bước 3 - Authentication:**
- Backend: `AuthController.cs` → POST `/api/auth/login`
- Verify username/password trong database
- Tạo JWT token với thông tin user
- Trả về response: `{ success: true, data: { token, role, username } }`

**Bước 4 - Lưu token:**
- Frontend JavaScript lưu vào `localStorage`:
  - `localStorage.setItem('token', response.data.token)`
  - `localStorage.setItem('role', response.data.role)`
  - `localStorage.setItem('username', response.data.username)`

**Bước 5 - Role-based Routing:**
- JavaScript kiểm tra role và redirect:
  ```javascript
  if (role === 'Admin') → window.location.href = 'admin/dashboard.html'
  if (role === 'HocVien') → window.location.href = 'hocvien/dashboard.html'
  if (role === 'HuanLuyenVien') → window.location.href = 'huanluyenvien/dashboard.html'
  ```

**Bước 6 - Dashboard theo Actor:**
- **Admin Dashboard:** `frontend/admin/dashboard.html`
  - Thống kê tổng quan
  - Menu điều hướng đến các trang quản lý
  - Giao diện: Copy từ `Demo_GiaoDien/Admin_ui/dashboard.html`
  
- **Học viên Dashboard:** `frontend/hocvien/dashboard.html`
  - Thông tin cá nhân
  - Menu điều hướng đến các trang học viên
  - Giao diện: Copy từ `Demo_GiaoDien/HocVien_ui/` (nếu có)
  
- **HLV Dashboard:** `frontend/huanluyenvien/dashboard.html`
  - Thông tin cá nhân
  - Menu điều hướng đến các trang HLV
  - Giao diện: Copy từ `Demo_GiaoDien/HLV_ui/` (nếu có)

---

## 📁 Cấu trúc thư mục

```
MartialArtsClubManagement/
├── backend/                                          # Backend ASP.NET Core API
│   └── MartialArtsClubManagement/
│       └── MartialArtsClubManagement.API/
│           ├── Controllers/                          # API Controllers
│           ├── Models/                               # Entities, DTOs
│           ├── Services/                             # Business logic
│           ├── Program.cs                            # Configuration
│           └── appsettings.json                      # Connection strings
│
├── frontend/                                         # Frontend (HTML/CSS/JS)
│   ├── index.html                                    # Landing page
│   ├── login.html                                    # Trang đăng nhập chung
│   ├── admin/                                        # Giao diện Admin
│   │   ├── dashboard.html
│   │   ├── tai-khoan.html
│   │   ├── hoc-vien.html
│   │   ├── huan-luyen-vien.html
│   │   ├── cap-dai.html
│   │   ├── khoa-hoc.html
│   │   ├── lop-hoc.html
│   │   ├── dang-ky-lop.html
│   │   ├── diem-danh.html
│   │   ├── hoc-phi.html
│   │   ├── thi-thang-dai.html
│   │   ├── goi-kem-rieng.html
│   │   ├── dang-ky-kem.html
│   │   └── thong-bao.html
│   │
│   ├── hocvien/                                      # Giao diện Học viên
│   │   ├── dashboard.html
│   │   ├── profile.html
│   │   ├── diem-danh.html
│   │   ├── thi.html
│   │   ├── lich-hoc.html
│   │   ├── hoc-phi.html
│   │   ├── thong-bao.html
│   │   └── doi-mat-khau.html
│   │
│   ├── huanluyenvien/                                # Giao diện Huấn luyện viên
│   │   ├── dashboard.html
│   │   ├── profile.html
│   │   ├── lich-day.html
│   │   ├── hoc-vien.html
│   │   ├── diem-danh.html
│   │   ├── thi.html
│   │   ├── thong-bao.html
│   │   └── doi-mat-khau.html
│   │
│   ├── css/                                          # Stylesheets
│   │   ├── style.css
│   │   ├── admin.css
│   │   ├── hocvien.css
│   │   ├── huanluyenvien.css
│   │   └── bootstrap.min.css
│   │
│   ├── js/                                           # JavaScript logic
│   │   ├── api.js                                   # Hàm gọi API chung
│   │   ├── auth.js                                  # Authentication logic
│   │   ├── main.js                                  # Hàm tiện ích
│   │   ├── admin.js                                 # Logic Admin
│   │   ├── hocvien.js                               # Logic Học viên
│   │   └── huanluyenvien.js                         # Logic HLV
│   │
│   └── assets/                                      # Static assets
│       ├── images/
│       ├── icons/
│       └── fonts/
│
└── Demo_GiaoDien/                                    # Giao diện mẫu
    ├── Admin_ui/
    ├── HocVien_ui/
    └── HLV_ui/
```

---

## 💻 Yêu cầu hệ thống

### Backend
- **.NET 10 SDK** hoặc cao hơn
- **SQL Server** (Express hoặc Standard)
- **Visual Studio 2022** hoặc VS Code

### Frontend
- **Trình duyệt web hiện đại** (Chrome, Firefox, Edge)
- **Không cần cài đặt framework** (HTML/CSS/JS thuần)
- **Bootstrap 5** (CDN hoặc local)

---

## 🚀 Cài đặt và thiết lập

### Bước 1: Clone repository
```bash
git clone https://github.com/nguyendinhvan-dev/MartialArtsClubManagement.git
cd MartialArtsClubManagement
```

### Bước 2: Thiết lập Backend
```bash
cd backend/MartialArtsClubManagement/MartialArtsClubManagement.API
dotnet restore
dotnet build
```

### Bước 3: Cấu hình Database
Mở file `appsettings.json` và cập nhật connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=QuanLyCLBVoThuat;Integrated Security=True;Trust Server Certificate=True"
  }
}
```

Chạy migration để tạo database:
```bash
dotnet ef database update
```

Hoặc chạy script SQL thủ công:
```bash
sqlcmd -S YOUR_SERVER -d QuanLyCLBVoThuat -i Nhom7_225LTC_Sharp.sql
```

### Bước 4: Chạy Backend API
```bash
dotnet run
```
API sẽ chạy tại: `https://localhost:5001` hoặc `http://localhost:5000`

### Bước 5: Thiết lập Frontend
```bash
cd ../../..
# Tạo thư mục frontend nếu chưa có
mkdir frontend
cd frontend
```

Copy các file từ `Demo_GiaoDien/` vào cấu trúc thư mục `frontend/` theo hướng dẫn ở trên.

### Bước 6: Cấu hình CORS trong Backend
Mở file `Program.cs` và thêm CORS policy:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

## 🔗 Tích hợp Frontend với Backend API

### Cấu hình API
- File `js/api.js`: Cấu hình URL backend, hàm gọi API chung (GET, POST, PUT, DELETE)
- File `js/auth.js`: Xử lý đăng nhập, đăng xuất, lưu token, kiểm tra authentication
- File `js/main.js`: Hàm tiện ích chung (format date, currency, notification)
- File `js/admin.js`: Logic riêng cho Admin
- File `js/hocvien.js`: Logic riêng cho Học viên
- File `js/huanluyenvien.js`: Logic riêng cho HLV

### Quy trình gọi API
1. Frontend JavaScript gọi API bằng `fetch()` hoặc hàm trong `api.js`
2. Gửi JWT token trong header: `Authorization: Bearer <token>`
3. Backend verify token, xử lý request
4. Backend trả về JSON response
5. Frontend render data vào HTML

---

## 📝 Hướng dẫn phát triển

### Tạo trang mới cho Admin
1. Tạo file HTML trong `frontend/admin/`
2. Copy giao diện từ `Demo_GiaoDien/Admin_ui/`
3. Thêm JavaScript để gọi API tương ứng
4. Gọi API từ `/api/admin/*`

### Tạo trang mới cho Học viên
1. Tạo file HTML trong `frontend/hocvien/`
2. Copy giao diện từ `Demo_GiaoDien/HocVien_ui/` (nếu có)
3. Thêm JavaScript để gọi API tương ứng
4. Gọi API từ `/api/HocVienPortal/*`

### Tạo trang mới cho HLV
1. Tạo file HTML trong `frontend/huanluyenvien/`
2. Copy giao diện từ `Demo_GiaoDien/HLV_ui/` (nếu có)
3. Thêm JavaScript để gọi API tương ứng
4. Gọi API từ `/api/HuanLuyenVienPortal/*`

---

## 🔧 Troubleshooting

### Lỗi 1: CORS Error
**Symptom:** Console hiển thị lỗi CORS khi gọi API

**Solution:**
1. Kiểm tra CORS policy trong `Program.cs`
2. Đảm bảo origin của frontend được cho phép
3. Restart backend server

### Lỗi 2: 401 Unauthorized
**Symptom:** API trả về 401 khi gọi với token

**Solution:**
1. Kiểm tra token trong localStorage
2. Đảm bảo token được gửi trong header: `Authorization: Bearer <token>`
3. Kiểm tra token có hết hạn không

### Lỗi 3: 403 Forbidden
**Symptom:** API trả về 403 khi truy cập

**Solution:**
1. Kiểm tra role của user
2. Đảm bảo user có quyền truy cập endpoint
3. Kiểm tra `[Authorize(Roles = "...")]` attribute trong Controller

### Lỗi 4: Database Connection Error
**Symptom:** Không thể kết nối database

**Solution:**
1. Kiểm tra connection string trong `appsettings.json`
2. Đảm bảo SQL Server đang chạy
3. Kiểm tra user có quyền truy cập database không

### Lỗi 5: Frontend không load được data
**Symptom:** Trang hiển thị nhưng không có dữ liệu

**Solution:**
1. Mở Developer Tools (F12) → Console để xem lỗi
2. Kiểm tra API endpoint có đúng không
3. Test API với Swagger UI trước
4. Kiểm tra JavaScript có lỗi không

---

## 📞 Hỗ trợ

Nếu gặp vấn đề:
1. Kiểm tra Console của trình duyệt (F12)
2. Test API với Swagger UI: `https://localhost:5001/swagger`
3. Kiểm tra log của Backend API
4. Liên hệ team phát triển

---

## 📚 Tài liệu tham khảo

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.0/)
- [MDN Web Docs - JavaScript](https://developer.mozilla.org/en-US/docs/Web/JavaScript)
- [Fetch API](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API)

---

## 📝 Changelog

### Version 1.0 (2026-05-28)
- Khởi tạo documentation
- Cấu trúc thư mục frontend
- Hướng dẫn tích hợp API
- Ví dụ code cơ bản

---

**Last updated:** 2026-05-28
**Author:** Team Development
