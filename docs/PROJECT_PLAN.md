# Kế Hoạch Hoàn Thiết Dự Án CLB Võ Thuật (4 Tuần) - Actor-Based

## 📋 Tổng Quan Dự Án

**Mục tiêu**: Xây dựng backend API và tích hợp với frontend demo hiện có
**Thời gian**: 4 tuần (28 ngày)
**Team size**: 3 người
**Phương pháp**: Mỗi người phát triển 1 actor riêng biệt
**Scope**: Backend ASP.NET Core + Database + API Integration

## 🎭 3 Actors Chính Trong Hệ Thống

### 👨‍💼 Actor 1: ADMIN (Quản Trị Viên)
- **Vai trò**: Quản lý toàn bộ hệ thống
- **Quyền**: Tất cả (CRUD mọi entity, quản lý user, system config)
- **Modules**: Dashboard, User Management, System Settings, Reports

### 👨‍🏫 Actor 2: HUẤN LUYỆN VIÊN (HLV)
- **Vai trò**: Quản lý lớp học, điểm danh, thi cử
- **Quyền**: Quản lý lớp, điểm danh, thi thăng đai, dạy kèm
- **Modules**: Class Management, Attendance, Grading, Private Training

### 👨‍🎓 Actor 3: HỌC VIÊN (HV)
- **Vai trò**: Xem thông tin cá nhân, đăng ký, học phí
- **Quyền**: Xem thông tin, đăng ký lớp, xem lịch, thanh toán
- **Modules**: Profile Registration, Class Enrollment, Payment, Progress

## 👥 Phân Công Theo Actor

### 🎯 Person 1: ADMIN Module Developer
- **Actor phụ trách**: ADMIN (Quản trị viên)
- **Chuyên môn**: System Architecture, Admin APIs, Security
- **Modules phát triển**:
  - Authentication & Authorization
  - User Management (Admin, HLV, HV)
  - System Configuration
  - Dashboard & Reports
  - Database Management

### 🎯 Person 2: HUẤN LUYỆN VIÊN Module Developer  
- **Actor phụ trách**: HUẤN LUYỆN VIÊN
- **Chuyên môn**: Education Logic, Class Management, Assessment
- **Modules phát triển**:
  - Lớp học & Khóa học
  - Điểm danh & Attendance
  - Thi thăng đai & Evaluation
  - Dạy kèm riêng
  - Lịch làm việc HLV

### 🎯 Person 3: HỌC VIÊN Module Developer
- **Actor phụ trách**: HỌC VIÊN  
- **Chuyên môn**: Student Experience, Registration, Payment
- **Modules phát triển**:
  - Profile & Registration
  - Class Enrollment
  - Học phí & Payment
  - Progress Tracking
  - Sự kiện & Thông báo

## 🔗 Integration Points (Điểm giao thoa)

### 🤝 Common Modules (Cả 3 cùng làm):
- **Authentication**: Person 1 lead, others support
- **Database Schema**: Person 1 setup, others use
- **API Documentation**: Tất cả cùng maintain
- **Testing**: Mỗi người test actor của mình + cross-test

### 🔄 Data Flow Between Actors:
```
ADMIN ←→ HLV ←→ HỌC VIÊN
   ↓        ↓        ↓
SYSTEM   CLASSES   PAYMENTS
```

---

## 📅 Kế Hoạch Chi Tiết 4 Tuần - Actor-Based

### 🗓️ Tuần 1: Foundation & Core Setup (Days 1-7)

#### 🎯 Mục tiêu tuần 1:
- Setup backend project structure chung
- Database connection và migrations
- Authentication system (Person 1 lead)
- Mỗi người setup entities cho actor của mình

#### 📋 Tasks chi tiết:

**👨‍💼 Person 1 (ADMIN Module):**
- [ ] Day 1: Create ASP.NET Core Web API project (shared)
- [ ] Day 2: Setup Entity Framework + Database (shared)
- [ ] Day 3: Implement JWT Authentication System
- [ ] Day 4: Create Admin entities (TaiKhoan, HocVien, HuanLuyenVien)
- [ ] Day 5: AuthController (Login/Register/Role Management)
- [ ] Day 6: Admin Dashboard APIs
- [ ] Day 7: User Management APIs + Documentation

**👨‍🏫 Person 2 (HUẤN LUYỆN VIÊN Module):**
- [ ] Day 1: Study existing database schema
- [ ] Day 2: Create HLV entities (LopHoc, KhoaHoc, DiemDanh)
- [ ] Day 3: Create HLV DTOs + Validation
- [ ] Day 4: Setup AutoMapper for HLV entities
- [ ] Day 5: LopHocController (CRUD)
- [ ] Day 6: DiemDanhController (Attendance)
- [ ] Day 7: KhoaHocController + Testing

**👨‍🎓 Person 3 (HỌC VIÊN Module):**
- [ ] Day 1: Study existing frontend HTML structure
- [ ] Day 2: Create HV entities (DangKyLop, GoiKemRieng, DangKyKem)
- [ ] Day 3: Create HV DTOs + Validation
- [ ] Day 4: Setup AutoMapper for HV entities
- [ ] Day 5: DangKyLopController (Enrollment)
- [ ] Day 6: GoiKemRiengController (Private Training)
- [ ] Day 7: Frontend integration prep + API testing

#### ✅ Deliverables Tuần 1:
- [ ] Backend API project structure
- [ ] Database connected with EF migrations
- [ ] JWT authentication working
- [ ] Basic CRUD APIs cho mỗi actor
- [ ] Postman collections cho mỗi module

---

### 🗓️ Tuần 2: Advanced APIs & Business Logic (Days 8-14)

#### 🎯 Mục tiêu tuần 2:
- Hoàn thành APIs chuyên sâu cho mỗi actor
- Implement business logic riêng
- Bắt đầu integration giữa actors

#### 📋 Tasks chi tiết:

**👨‍💼 Person 1 (ADMIN Module):**
- [ ] Day 8: System Configuration APIs
- [ ] Day 9: Report & Analytics APIs
- [ ] Day 10: CapDai Management APIs
- [ ] Day 11: ThongBao Management APIs
- [ ] Day 12: Admin Dashboard Service Layer
- [ ] Day 13: Cross-actor integration (Admin ↔ HLV)
- [ ] Day 14: Admin module testing + Documentation

**👨‍🏫 Person 2 (HUẤN LUYỆN VIÊN Module):**
- [ ] Day 8: KyThiThangDai APIs (Exam Management)
- [ ] Day 9: KetQuaThi APIs (Grading System)
- [ ] Day 10: LichLamViec HLV APIs
- [ ] Day 11: DangKyKem APIs (Private Training)
- [ ] Day 12: HLV Service Layer (Business Rules)
- [ ] Day 13: Cross-actor integration (HLV ↔ Học Viên)
- [ ] Day 14: HLV module testing + Frontend integration

**👨‍🎓 Person 3 (HỌC VIÊN Module):**
- [ ] Day 8: HocPhi APIs (Payment System)
- [ ] Day 9: ThanhToan APIs (Payment Processing)
- [ ] Day 10: SuKien APIs (Event Registration)
- [ ] Day 11: DangKySuKien APIs
- [ ] Day 12: HV Service Layer (Student Logic)
- [ ] Day 13: Cross-actor integration (Học Viên ↔ Admin)
- [ ] Day 14: HV module testing + Frontend integration

---

### 🗓️ Tuần 3: Full Integration & Advanced Features (Days 15-21)

#### 🎯 Mục tiêu tuần 3:
- Hoàn thiện frontend integration
- Complex business logic giữa actors
- Performance optimization
- Advanced features

#### 📋 Tasks chi tiết:

**👨‍💼 Person 1 (ADMIN Module):**
- [ ] Day 15: Admin Dashboard frontend integration
- [ ] Day 16: User Management UI integration
- [ ] Day 17: System Reports frontend
- [ ] Day 18: Cross-actor data validation
- [ ] Day 19: Admin module performance optimization
- [ ] Day 20: Security hardening (Admin features)
- [ ] Day 21: Admin module UAT + Documentation

**👨‍🏫 Person 2 (HUẤN LUYỆN VIÊN Module):**
- [ ] Day 15: LopHoc Management UI integration
- [ ] Day 16: DiemDanh real-time UI
- [ ] Day 17: Thi thăng đai UI integration
- [ ] Day 18: Dạy kèm management UI
- [ ] Day 19: HLV module performance optimization
- [ ] Day 20: Business rules validation
- [ ] Day 21: HLV module UAT + User guide

**👨‍🎓 Person 3 (HỌC VIÊN Module):**
- [ ] Day 15: Student Profile UI integration
- [ ] Day 16: Class Enrollment UI
- [ ] Day 17: Payment System UI
- [ ] Day 18: Event Registration UI
- [ ] Day 19: HV module performance optimization
- [ ] Day 20: Payment flow testing
- [ ] Day 21: HV module UAT + User guide

#### ✅ Deliverables Tuần 3:
- [ ] Full frontend integration for all actors
- [ ] Complex business logic working
- [ ] Performance optimized
- [ ] Cross-actor data flow seamless
- [ ] User acceptance testing completed

---

### 🗓️ Tuần 4: Testing, Deployment & Documentation (Days 22-28)

#### 🎯 Mục tiêu tuần 4:
- End-to-end testing
- Deployment preparation
- Final documentation
- Demo preparation

#### 📋 Tasks chi tiết:

**👨‍💼 Person 1 (ADMIN Module):**
- [ ] Day 22: Admin module end-to-end testing
- [ ] Day 23: System integration testing
- [ ] Day 24: Deployment configuration
- [ ] Day 25: Production database setup
- [ ] Day 26: Admin documentation final
- [ ] Day 27: System monitoring setup
- [ ] Day 28: Final demo preparation

**👨‍🏫 Person 2 (HUẤN LUYỆN VIÊN Module):**
- [ ] Day 22: HLV module end-to-end testing
- [ ] Day 23: Cross-actor workflow testing
- [ ] Day 24: Data migration scripts
- [ ] Day 25: HLV documentation final
- [ ] Day 26: User manual creation
- [ ] Day 27: Testing report compilation
- [ ] Day 28: Demo rehearsal

**👨‍🎓 Person 3 (HỌC VIÊN Module):**
- [ ] Day 22: HV module end-to-end testing
- [ ] Day 23: Payment system testing
- [ ] Day 24: Production build optimization
- [ ] Day 25: HV documentation final
- [ ] Day 26: Video tutorial creation
- [ ] Day 27: User guide finalization
- [ ] Day 28: Final presentation

#### ✅ Deliverables Tuần 4:
- [ ] Production-ready application
- [ ] Complete documentation
- [ ] Deployment scripts
- [ ] User manuals for each actor
- [ ] Final demo presentation

---

## �️ API Structure Theo Actor

### 👨‍💼 ADMIN APIs (Person 1):
```
/api/auth/*                    - Authentication & Authorization
/api/admin/users/*             - User Management
/api/admin/dashboard/*         - Dashboard & Analytics
/api/admin/system/*            - System Configuration
/api/admin/reports/*           - Reports & Statistics
/api/admin/capdai/*            - Cấp Đai Management
/api/admin/thongbao/*          - Thông Báo System
```

### 👨‍🏫 HUẤN LUYỆN VIÊN APIs (Person 2):
```
/api/hlv/lophoc/*              - Lớp Học Management
/api/hlv/khoahoc/*             - Khóa Học Management  
/api/hlv/diemdanh/*            - Điểm Danh System
/api/hlv/kythi/*               - Kỳ Thi Thăng Đai
/api/hlv/ketquathi/*           - Kết Quả Thi
/api/hlv/dangkykem/*           - Đăng Ký Dạy Kèm
/api/hlv/lichlamviec/*          - Lịch Làm Việc
```

### 👨‍🎓 HỌC VIÊN APIs (Person 3):
```
/api/hv/profile/*              - Student Profile
/api/hv/dangkylop/*            - Class Enrollment
/api/hv/hocphi/*               - Học Phí Management
/api/hv/thanhtoan/*            - Payment Processing
/api/hv/goikem/*               - Gói Kèm Riêng
/api/hv/sukien/*               - Sự Kiện & Thông Báo
/api/hv/dangkysukien/*         - Event Registration
/api/hv/tiendo/*               - Progress Tracking
```

---

## 🔗 Cross-Actor Integration Points

### 🤝 Shared Data Flow:
1. **User Authentication** (Admin lead) → All actors use
2. **Class Enrollment** (HV) → Updates HLV class lists
3. **Payment Processing** (HV) → Updates Admin financial reports
4. **Exam Results** (HLV) → Updates HV progress + Admin reports
5. **Attendance** (HLV) → Updates HV records + Admin analytics

### � Integration Schedule:
- **Week 2**: Basic cross-actor APIs
- **Week 3**: Complex workflows
- **Week 4**: End-to-end testing

---

## 📊 Success Metrics Theo Actor

### �‍💼 Admin Module Success:
- [ ] All user management working
- [ ] Dashboard real-time data
- [ ] Reports accurate
- [ ] System configuration functional

### 👨‍🏫 HLV Module Success:
- [ ] Class management seamless
- [ ] Attendance tracking accurate
- [ ] Exam grading system working
- [ ] Private training scheduling functional

### 👨‍🎓 HV Module Success:
- [ ] Registration process smooth
- [ ] Payment system working
- [ ] Progress tracking accurate
- [ ] Event registration functional

---

## 🚨 Risk Management Theo Actor

### 👨‍💼 Admin Risks:
- **Security breaches** → JWT + input validation
- **Data corruption** → Database transactions
- **Performance issues** → Caching + optimization

### 👨‍🏫 HLV Risks:
- **Complex business rules** → Clear validation
- **Real-time attendance** → WebSocket consideration
- **Exam integrity** → Secure grading system

### 👨‍🎓 HV Risks:
- **Payment failures** → Error handling + retries
- **Registration conflicts** → Validation rules
- **User experience** → Responsive design

---

**Actor-based approach ready! 🎭**

---

## 🔧 Công Cụ và Công Nghệ

### Backend Stack:
- **.NET 6** hoặc **.NET 7**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **FluentValidation**
- **AutoMapper**

### Frontend Integration:
- **JavaScript ES6+**
- **Bootstrap 5**
- **Axios** for API calls
- **Postman** for testing

### Development Tools:
- **Visual Studio 2022**
- **Git** với workflow đã setup
- **Postman**
- **SQL Server Management Studio**

---

## 📊 Progress Tracking

### Daily Standup Template:
1. **Hôm qua làm được gì?**
2. **Hôm nay làm gì?**
3. **Có blocker nào không?**

### Weekly Review:
- **Monday**: Planning cho tuần
- **Friday**: Demo và retrospective
- **Daily**: 15-minute standup

### Metrics:
- **API completion rate**
- **Test coverage percentage**
- **Bug count and resolution time**
- **Integration progress**

---

## 🚨 Risk Management

### Technical Risks:
- **Database connection issues** → Person 1负责
- **API performance problems** → Person 1 + Person 2
- **Frontend integration bugs** → Person 3

### Timeline Risks:
- **Scope creep** → Strict requirement management
- **Technical debt** → Code review every day
- **Team coordination** → Daily standups + clear communication

### Mitigation:
- **Daily code reviews**
- **Automated testing**
- **Continuous integration**
- **Backup plans for critical tasks**

---

## 🎯 Success Criteria

### Technical:
- [ ] All APIs working correctly
- [ ] Frontend fully integrated
- [ ] Performance acceptable (<2s response time)
- [ ] Security implemented (JWT, input validation)

### Process:
- [ ] On-time delivery
- [ ] Code quality maintained
- [ ] Documentation complete
- [ ] Team collaboration effective

### User:
- [ ] All features working as expected
- [ ] User-friendly interface
- [ ] No major bugs
- [ ] Successful demo presentation

---

## 📞 Communication Plan

### Daily:
- **9:00 AM** - Standup (15 minutes)
- **Continuous** - Slack/Teams for questions

### Weekly:
- **Monday 9:30 AM** - Week planning (30 minutes)
- **Friday 4:00 PM** - Demo and review (1 hour)

### Escalation:
- **Technical blockers** → Immediate team discussion
- **Timeline issues** → Group decision within 24 hours
- **Scope changes** → Team approval required

---

**Ready to start! 🚀**
