-- ============================================================
--  DATABASE: Quản lý Câu lạc bộ Võ thuật
--  Ngôn ngữ: SQL Server (T-SQL)
--  Mô tả: Script tạo toàn bộ cơ sở dữ liệu cho hệ thống
--         quản lý CLB võ thuật chuyên sâu một môn.
--         Bao gồm 14 bảng, quan hệ, ràng buộc và dữ liệu mẫu.
--  Phiên bản: v3 – Sửa lỗi thứ tự tạo bảng (FK CapDai trong HocVien)
--             + Thêm IF EXISTS DROP để script có thể chạy lại an toàn
-- ============================================================

-- ============================================================
-- BƯỚC 0: XÓA DATABASE CŨ NẾU ĐÃ TỒN TẠI (chạy lại an toàn)
-- ============================================================
USE master;
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QuanLyCLBVoThuat')
BEGIN
    -- Đóng toàn bộ kết nối đang mở vào DB trước khi DROP
    ALTER DATABASE QuanLyCLBVoThuat SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyCLBVoThuat;
END
GO

CREATE DATABASE QuanLyCLBVoThuat;
GO
USE QuanLyCLBVoThuat;
GO


-- ============================================================
-- NHÓM 1: TÀI KHOẢN & NGƯỜI DÙNG
-- Lý do thiết kế:
--   Tách thành 3 bảng (TaiKhoan, HocVien, HuanLuyenVien) thay
--   vì gộp chung vào một bảng vì mỗi vai trò có thông tin riêng
--   biệt. TaiKhoan chứa thông tin đăng nhập dùng chung, còn
--   HocVien và HuanLuyenVien chứa thông tin nghiệp vụ riêng.
--   Đây là pattern "Table per Type" giúp tránh cột NULL thừa.
-- ============================================================

-- Bảng TaiKhoan
-- Lưu thông tin đăng nhập cho tất cả người dùng hệ thống.
-- VaiTro phân biệt 3 loại: 'QuanTriVien', 'HuanLuyenVien', 'HocVien'
-- giúp phân quyền truy cập tại tầng middleware của ASP.NET Core.
IF OBJECT_ID('TaiKhoan', 'U') IS NOT NULL DROP TABLE TaiKhoan;
CREATE TABLE TaiKhoan (
    MaTaiKhoan      INT             PRIMARY KEY IDENTITY(1,1),
    HoTen           NVARCHAR(100)   NOT NULL,
    Email           VARCHAR(150)    NOT NULL UNIQUE,
    MatKhauHash     VARCHAR(256)    NOT NULL,
    -- VaiTro dùng CHECK constraint thay vì bảng riêng
    -- vì số lượng role cố định, không cần mở rộng
    VaiTro          VARCHAR(20)     NOT NULL CHECK (VaiTro IN ('QuanTriVien','HuanLuyenVien','HocVien')),
    DangHoatDong    BIT             NOT NULL DEFAULT 1,
    NgayTao         DATETIME        NOT NULL DEFAULT GETDATE()
);
GO

-- Bảng HuanLuyenVien
-- Lưu thông tin chi tiết của huấn luyện viên.
-- Tương tự HocVien, quan hệ 1-1 với TaiKhoan.
-- ChuyenMon: ghi chú chuyên môn riêng (vd: "Quyền pháp", "Vũ khí")
-- dù CLB chỉ có 1 môn võ, HLV vẫn có thể có chuyên sâu khác nhau.
IF OBJECT_ID('HuanLuyenVien', 'U') IS NOT NULL DROP TABLE HuanLuyenVien;
CREATE TABLE HuanLuyenVien (
    MaHLV           INT             PRIMARY KEY IDENTITY(1,1),
    MaTaiKhoan      INT             NOT NULL UNIQUE,
    SoDienThoai     VARCHAR(15)     NULL,
    ChuyenMon       NVARCHAR(100)   NULL,
    NgayVaoCLB      DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    DangHoatDong    BIT             NOT NULL DEFAULT 1,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO


-- ============================================================
-- NHÓM 2: HỆ THỐNG ĐAI
-- Lý do thiết kế:
--   Lưu danh sách cấp đai vào bảng riêng (không hardcode) để
--   Admin có thể thêm/sửa cấp đai mà không cần sửa code.
--   ThuTu giúp sắp xếp đúng thứ bậc (Trắng < Vàng < ... < Đen).
--   ThoiGianToiThieuThang là điều kiện tối thiểu để học viên
--   được phép thi thăng đai lên cấp tiếp theo.
-- ============================================================

-- Bảng CapDai
-- 7 cấp mặc định: Trắng(1) → Vàng(2) → Cam(3) → Xanh lá(4)
--                 → Xanh dương(5) → Đỏ(6) → Đen(7)
-- ThuTu là số thứ tự để so sánh cấp bậc (đai cao hơn = ThuTu lớn hơn)
-- [ĐÃ SỬA v3] Bảng CapDai được tạo TRƯỚC HocVien để FK hợp lệ
IF OBJECT_ID('CapDai', 'U') IS NOT NULL DROP TABLE CapDai;
CREATE TABLE CapDai (
    MaCapDai                INT             PRIMARY KEY IDENTITY(1,1),
    TenCapDai               NVARCHAR(50)    NOT NULL,
    MauSac                  NVARCHAR(30)    NOT NULL,  -- Tên màu để hiển thị badge UI
    ThuTu                   INT             NOT NULL UNIQUE,  -- Dùng để sort và so sánh cấp bậc
    ThoiGianToiThieuThang   INT             NOT NULL DEFAULT 3  -- Số tháng tối thiểu ở đai này trước khi thi lên
);
GO

-- Bảng HocVien
-- Lưu thông tin chi tiết của học viên.
-- Quan hệ 1-1 với TaiKhoan: mỗi học viên có đúng 1 tài khoản.
-- MaCapDaiHienTai là FK đến CapDai – lưu cấp đai HIỆN TẠI của
-- học viên, giúp truy vấn nhanh mà không cần JOIN qua KetQuaThi.
-- Lịch sử thăng đai đầy đủ được lưu trong bảng KetQuaThi.
-- [ĐÃ SỬA v3] Bỏ FK CapDai inline (vì v2 khai báo khi CapDai chưa tồn tại).
--             FK được thêm bằng ALTER TABLE bên dưới sau khi CapDai đã có.
IF OBJECT_ID('HocVien', 'U') IS NOT NULL DROP TABLE HocVien;
CREATE TABLE HocVien (
    MaHocVien       INT             PRIMARY KEY IDENTITY(1,1),
    MaTaiKhoan      INT             NOT NULL UNIQUE,  -- UNIQUE đảm bảo 1-1
    SoDienThoai     VARCHAR(15)     NULL,
    DiaChi          NVARCHAR(200)   NULL,
    NgaySinh        DATE            NULL,
    GioiTinh        VARCHAR(5)      NULL CHECK (GioiTinh IN ('Nam','Nu')),
    NgayGiaNhap     DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    MaCapDaiHienTai INT             NULL,  -- NULL nếu chưa xếp đai (học viên mới)
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
    -- FK đến CapDai được thêm bằng ALTER TABLE bên dưới
);
GO

-- Thêm FK từ HocVien → CapDai
-- (CapDai đã tồn tại ở bước trên nên ALTER TABLE này luôn thành công)
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_HocVien_CapDai'
)
ALTER TABLE HocVien
    ADD CONSTRAINT FK_HocVien_CapDai
    FOREIGN KEY (MaCapDaiHienTai) REFERENCES CapDai(MaCapDai);
GO

-- Dữ liệu mẫu: 7 cấp đai
INSERT INTO CapDai (TenCapDai, MauSac, ThuTu, ThoiGianToiThieuThang) VALUES
    (N'Đai Trắng',      N'Trắng',       1, 3),
    (N'Đai Vàng',       N'Vàng',        2, 3),
    (N'Đai Cam',        N'Cam',         3, 3),
    (N'Đai Xanh lá',   N'Xanh lá',     4, 4),
    (N'Đai Xanh dương', N'Xanh dương',  5, 4),
    (N'Đai Đỏ',        N'Đỏ',          6, 6),
    (N'Đai Đen',       N'Đen',         7, 0);  -- Đai Đen không cần thi lên nữa
GO


-- ============================================================
-- NHÓM 3: KHÓA HỌC & LỚP HỌC
-- Lý do thiết kế:
--   Tách KhoaHoc và LopHoc thành 2 bảng vì:
--   - KhoaHoc là đơn vị tuyển sinh (có ngày khai giảng, học phí,
--     sĩ số tối đa, trạng thái mở/đóng)
--   - LopHoc là đơn vị giảng dạy cụ thể trong khóa (phân theo
--     cấp đai, có HLV và phòng tập riêng)
--   Một khóa học có thể có nhiều lớp (mỗi cấp đai 1 lớp).
-- ============================================================

-- Bảng KhoaHoc
-- TrangThai: 'ChuaMo' → 'DangTuyenSinh' → 'DangHoc' → 'DaKetThuc'
-- MoiKhoaLa3Thang: StartDate + 3 tháng = EndDate (enforce ở application layer)
-- HocPhi lưu ở KhoaHoc vì học phí áp dụng chung cho cả khóa,
-- không phân biệt lớp (tất cả lớp trong khóa cùng mức học phí).
IF OBJECT_ID('KhoaHoc', 'U') IS NOT NULL DROP TABLE KhoaHoc;
CREATE TABLE KhoaHoc (
    MaKhoaHoc       INT             PRIMARY KEY IDENTITY(1,1),
    TenKhoaHoc      NVARCHAR(100)   NOT NULL,
    NgayKhaiGiang   DATE            NOT NULL,
    NgayKetThuc     DATE            NOT NULL,
    HocPhi          DECIMAL(10,0)   NOT NULL,
    SoLuongToiDa    INT             NOT NULL DEFAULT 100,
    TrangThai       VARCHAR(20)     NOT NULL DEFAULT 'ChuaMo'
                    CHECK (TrangThai IN ('ChuaMo','DangTuyenSinh','DangHoc','DaKetThuc')),
    -- Đảm bảo ngày kết thúc sau ngày khai giảng
    CONSTRAINT CHK_KhoaHoc_NgayHopLe CHECK (NgayKetThuc > NgayKhaiGiang)
);
GO

-- Bảng LopHoc
-- Mỗi lớp thuộc 1 khóa, dành cho 1 cấp đai, do 1 HLV phụ trách.
-- LichHoc: lưu dạng text linh hoạt, vd "Thứ 2,4,6 - 18:00-19:30"
-- vì lịch học có thể phức tạp, không chuẩn hóa thành cột riêng
-- để tránh over-engineering ở phạm vi đề tài này.
-- UNIQUE(MaKhoaHoc, MaCapDai) đảm bảo mỗi khóa chỉ có
-- 1 lớp cho mỗi cấp đai, đúng với yêu cầu nghiệp vụ.
IF OBJECT_ID('LopHoc', 'U') IS NOT NULL DROP TABLE LopHoc;
CREATE TABLE LopHoc (
    MaLop           INT             PRIMARY KEY IDENTITY(1,1),
    MaKhoaHoc       INT             NOT NULL,
    MaCapDai        INT             NOT NULL,
    MaHLV           INT             NOT NULL,
    LichHoc         NVARCHAR(100)   NOT NULL,
    SoLuongToiDa    INT             NOT NULL DEFAULT 30,
    PhongTap        NVARCHAR(50)    NULL,
    FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc),
    FOREIGN KEY (MaCapDai)  REFERENCES CapDai(MaCapDai),
    FOREIGN KEY (MaHLV)     REFERENCES HuanLuyenVien(MaHLV),
    -- Mỗi khóa chỉ có 1 lớp cho mỗi cấp đai
    CONSTRAINT UQ_LopHoc_KhoaCapDai UNIQUE (MaKhoaHoc, MaCapDai)
);
GO


-- ============================================================
-- NHÓM 4: ĐĂNG KÝ LỚP & ĐIỂM DANH
-- Lý do thiết kế:
--   DangKyLop là bảng trung gian (junction table) giữa HocVien
--   và LopHoc, thể hiện quan hệ nhiều-nhiều. Ngoài ra nó còn
--   lưu trạng thái thanh toán học phí của từng học viên trong
--   từng lớp – tránh phải tạo bảng thanh toán riêng cho trường
--   hợp đơn giản này.
--   DiemDanh liên kết với DangKyLop (không phải trực tiếp với
--   HocVien và LopHoc) vì điểm danh chỉ có nghĩa với học viên
--   ĐÃ đăng ký lớp đó.
-- ============================================================

-- Bảng DangKyLop
-- TrangThaiThanhToan: 'ChuaThanhToan' | 'DaThanhToan' | 'MienGiam'
-- NgayThanhToan NULL khi chưa thanh toán.
-- UNIQUE(MaHocVien, MaLop) đảm bảo học viên không đăng ký
-- trùng cùng một lớp.
IF OBJECT_ID('DangKyLop', 'U') IS NOT NULL DROP TABLE DangKyLop;
CREATE TABLE DangKyLop (
    MaDangKy            INT             PRIMARY KEY IDENTITY(1,1),
    MaHocVien           INT             NOT NULL,
    MaLop               INT             NOT NULL,
    NgayDangKy          DATE            NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    TrangThaiThanhToan  VARCHAR(15)     NOT NULL DEFAULT 'ChuaThanhToan'
                        CHECK (TrangThaiThanhToan IN ('ChuaThanhToan','DaThanhToan','MienGiam')),
    NgayThanhToan       DATETIME        NULL,
    FOREIGN KEY (MaHocVien) REFERENCES HocVien(MaHocVien),
    FOREIGN KEY (MaLop)     REFERENCES LopHoc(MaLop),
    CONSTRAINT UQ_DangKyLop UNIQUE (MaHocVien, MaLop)
);
GO

-- Bảng DiemDanh
-- TrangThai: 'CóMặt' | 'VắngPhép' | 'VắngKhôngPhép'
-- MaDangKy FK đến DangKyLop để biết chính xác học viên nào,
-- lớp nào, buổi nào – tránh dư thừa dữ liệu.
-- UNIQUE(MaDangKy, NgayHoc) ngăn điểm danh trùng cùng buổi.
IF OBJECT_ID('DiemDanh', 'U') IS NOT NULL DROP TABLE DiemDanh;
CREATE TABLE DiemDanh (
    MaDiemDanh      INT             PRIMARY KEY IDENTITY(1,1),
    MaDangKy        INT             NOT NULL,
    NgayHoc         DATE            NOT NULL,
    TrangThai       NVARCHAR(20)    NOT NULL DEFAULT N'CóMặt'
                    CHECK (TrangThai IN (N'CóMặt', N'VắngPhép', N'VắngKhôngPhép')),
    GhiChu          NVARCHAR(200)   NULL,
    FOREIGN KEY (MaDangKy) REFERENCES DangKyLop(MaDangKy),
    CONSTRAINT UQ_DiemDanh UNIQUE (MaDangKy, NgayHoc)
);
GO


-- ============================================================
-- NHÓM 5: DẠY KÈM RIÊNG
-- Lý do thiết kế:
--   Tách thành GoiKemRieng và DangKyKem vì:
--   - GoiKemRieng là "sản phẩm" Admin tạo sẵn (tên gói, số buổi,
--     học phí) – giống như menu dịch vụ
--   - DangKyKem là lần đăng ký cụ thể của từng học viên, Admin
--     sẽ phân công HLV sau khi HV đăng ký
--   Gói kèm gắn với KhoaHoc để đảm bảo dạy kèm chỉ diễn ra
--   trong phạm vi khóa học đang hoạt động.
-- ============================================================

-- Bảng GoiKemRieng
-- DangSuDung: Admin có thể tắt gói cũ mà không cần xóa
-- (preserve historical data của các DangKyKem đã có).
IF OBJECT_ID('GoiKemRieng', 'U') IS NOT NULL DROP TABLE GoiKemRieng;
CREATE TABLE GoiKemRieng (
    MaGoi           INT             PRIMARY KEY IDENTITY(1,1),
    TenGoi          NVARCHAR(100)   NOT NULL,
    SoBuoi          INT             NOT NULL CHECK (SoBuoi > 0),
    HocPhi          DECIMAL(10,0)   NOT NULL,
    MaKhoaHoc       INT             NOT NULL,
    DangSuDung      BIT             NOT NULL DEFAULT 1,
    FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc)
);
GO

-- Bảng DangKyKem  [ĐÃ SỬA v2]
-- Luồng nghiệp vụ đúng:
--   Bước 1: Học viên đăng ký gói kèm qua hệ thống
--           → MaHLV = NULL, TrangThaiPhanCong = 'ChoPhanCong'
--   Bước 2: Admin xem danh sách chờ và phân công HLV phù hợp
--           → UPDATE MaHLV, TrangThaiPhanCong = 'DaPhanCong', NgayBatDau
--   Bước 3: Admin xác nhận thanh toán sau khi HV nộp tiền trực tiếp
--           → UPDATE TrangThaiThanhToan = 'DaThanhToan', NgayThanhToan
--
-- Lý do MaHLV cho phép NULL:
--   Bản gốc khai báo MaHLV NOT NULL, nghĩa là phải có HLV ngay
--   lúc HV đăng ký – điều này mâu thuẫn với use case vì HV không
--   tự chọn HLV, Admin mới là người phân công sau.
--
-- TrangThaiPhanCong giúp Admin lọc nhanh danh sách chờ xử lý.
-- NgayBatDau cho phép NULL vì chỉ xác định được sau khi Admin
-- phân công và thống nhất lịch với HLV.
IF OBJECT_ID('DangKyKem', 'U') IS NOT NULL DROP TABLE DangKyKem;
CREATE TABLE DangKyKem (
    MaDangKyKem         INT             PRIMARY KEY IDENTITY(1,1),
    MaHocVien           INT             NOT NULL,
    MaGoi               INT             NOT NULL,
    MaHLV               INT             NULL,      -- NULL cho đến khi Admin phân công
	-- Thêm cột NgayDangKy vào DangKyKem (trong CREATE TABLE):
	NgayDangKy  DATETIME  NOT NULL DEFAULT GETDATE(),
    TrangThaiPhanCong   VARCHAR(15)     NOT NULL DEFAULT 'ChoPhanCong'
                        CHECK (TrangThaiPhanCong IN ('ChoPhanCong','DaPhanCong','DaHuy')),
    NgayBatDau          DATE            NULL,      -- NULL cho đến khi Admin xác nhận lịch
    TrangThaiThanhToan  VARCHAR(15)     NOT NULL DEFAULT 'ChuaThanhToan'
                        CHECK (TrangThaiThanhToan IN ('ChuaThanhToan','DaThanhToan','MienGiam')),
    NgayThanhToan       DATETIME        NULL,
    FOREIGN KEY (MaHocVien) REFERENCES HocVien(MaHocVien),
    FOREIGN KEY (MaGoi)     REFERENCES GoiKemRieng(MaGoi),
    FOREIGN KEY (MaHLV)     REFERENCES HuanLuyenVien(MaHLV),
    CONSTRAINT UQ_DangKyKem UNIQUE (MaHocVien, MaGoi)
);
GO


-- ============================================================
-- NHÓM 6: THI THĂNG ĐAI
-- Lý do thiết kế:
--   KyThiThangDai gắn với KhoaHoc vì CLB tổ chức thi theo đợt
--   trong từng khóa (cuối khóa thường có 1 kỳ thi).
--   KetQuaThi lưu điểm số và kết quả từng học viên.
--   MaCapDaiMoi chỉ có giá trị khi DaDat = 1 (học viên đạt).
--   Khi học viên đạt, hệ thống cập nhật MaCapDaiHienTai trong
--   bảng HocVien – đây là "single source of truth" cho cấp đai
--   hiện tại, giúp query nhanh mà không cần tính toán lại.
-- ============================================================

-- Bảng KyThiThangDai
-- TrangThai: 'SapDienRa' | 'DaDienRa' | 'DaHuy'
-- Một khóa học có thể có nhiều kỳ thi (thi giữa khóa, cuối khóa)
-- nên quan hệ KhoaHoc 1-n KyThi là hợp lý.
IF OBJECT_ID('KyThiThangDai', 'U') IS NOT NULL DROP TABLE KyThiThangDai;
CREATE TABLE KyThiThangDai (
    MaKyThi         INT             PRIMARY KEY IDENTITY(1,1),
    MaKhoaHoc       INT             NOT NULL,
    NgayThi         DATE            NOT NULL,
    MoTa            NVARCHAR(300)   NULL,
    TrangThai       VARCHAR(15)     NOT NULL DEFAULT 'SapDienRa'
                    CHECK (TrangThai IN ('SapDienRa','DaDienRa','DaHuy')),
    FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc)
);
GO

-- Bảng KetQuaThi
-- DiemSo: thang điểm 10, lưu DECIMAL để có thể có điểm lẻ.
-- DaDat: BIT – true nếu đủ điểm qua kỳ thi.
-- MaCapDaiMoi: FK đến CapDai – cấp đai học viên được thăng lên.
--   NULL nếu học viên không đạt.
-- UNIQUE(MaKyThi, MaHocVien) đảm bảo mỗi học viên chỉ có
-- 1 kết quả cho mỗi kỳ thi.
IF OBJECT_ID('KetQuaThi', 'U') IS NOT NULL DROP TABLE KetQuaThi;
CREATE TABLE KetQuaThi (
    MaKetQua        INT             PRIMARY KEY IDENTITY(1,1),
    MaKyThi         INT             NOT NULL,
    MaHocVien       INT             NOT NULL,
    DiemSo          DECIMAL(4,1)    NOT NULL CHECK (DiemSo >= 0 AND DiemSo <= 10),
    DaDat           BIT             NOT NULL DEFAULT 0,
    MaCapDaiMoi     INT             NULL,  -- NULL nếu không đạt
    FOREIGN KEY (MaKyThi)     REFERENCES KyThiThangDai(MaKyThi),
    FOREIGN KEY (MaHocVien)   REFERENCES HocVien(MaHocVien),
    FOREIGN KEY (MaCapDaiMoi) REFERENCES CapDai(MaCapDai),
    CONSTRAINT UQ_KetQuaThi UNIQUE (MaKyThi, MaHocVien)
);
GO


-- ============================================================
-- NHÓM 7: THÔNG BÁO & SỰ KIỆN
-- Lý do thiết kế:
--   Gộp thông báo và sự kiện vào một bảng ThongBao vì cả hai
--   đều có cấu trúc tương tự (tiêu đề, nội dung, ngày đăng).
--   LoaiThongBao phân biệt 'ThongBao' thường (không cần đăng ký)
--   và 'SuKien' (có thể đăng ký tham gia qua DangKySuKien).
--   Thiết kế này tránh phải tạo 2 bảng gần như giống nhau.
-- ============================================================

-- Bảng ThongBao
-- MaTaiKhoanTao FK đến TaiKhoan (không phải HuanLuyenVien/Admin)
-- vì người tạo được xác định qua TaiKhoan, linh hoạt hơn.
IF OBJECT_ID('ThongBao', 'U') IS NOT NULL DROP TABLE ThongBao;
CREATE TABLE ThongBao (
    MaThongBao      INT             PRIMARY KEY IDENTITY(1,1),
    MaTaiKhoanTao   INT             NOT NULL,
    TieuDe          NVARCHAR(200)   NOT NULL,
    NoiDung         NVARCHAR(MAX)   NOT NULL,
    LoaiThongBao    VARCHAR(15)     NOT NULL DEFAULT 'ThongBao'
                    CHECK (LoaiThongBao IN ('ThongBao','SuKien')),
    NgayDang        DATETIME        NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MaTaiKhoanTao) REFERENCES TaiKhoan(MaTaiKhoan)
);
GO

-- Bảng DangKySuKien
-- Chỉ áp dụng với ThongBao có LoaiThongBao = 'SuKien'.
-- Ràng buộc này được kiểm soát ở application layer (không dùng
-- CHECK constraint vì cần JOIN sang ThongBao để kiểm tra LoaiThongBao).
-- UNIQUE(MaThongBao, MaHocVien) ngăn đăng ký trùng.
IF OBJECT_ID('DangKySuKien', 'U') IS NOT NULL DROP TABLE DangKySuKien;
CREATE TABLE DangKySuKien (
    MaDangKySuKien  INT             PRIMARY KEY IDENTITY(1,1),
    MaThongBao      INT             NOT NULL,
    MaHocVien       INT             NOT NULL,
    NgayDangKy      DATETIME        NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MaThongBao) REFERENCES ThongBao(MaThongBao),
    FOREIGN KEY (MaHocVien)  REFERENCES HocVien(MaHocVien),
    CONSTRAINT UQ_DangKySuKien UNIQUE (MaThongBao, MaHocVien)
);
GO


-- ============================================================
-- DỮ LIỆU MẪU (SEED DATA) – dùng để test hệ thống
-- ============================================================

-- Tài khoản mẫu (mật khẩu đều là "Admin@123" – hash giả)
INSERT INTO TaiKhoan (HoTen, Email, MatKhauHash, VaiTro) VALUES
    (N'Nguyễn Văn Admin',   'admin@clb.vn',      'hash_admin',   'QuanTriVien'),
    (N'Trần Minh Tuấn',     'hlv1@clb.vn',       'hash_hlv1',    'HuanLuyenVien'),
    (N'Lê Thị Hoa',         'hlv2@clb.vn',       'hash_hlv2',    'HuanLuyenVien'),
    (N'Nguyễn Đình Văn',    'van@student.vn',    'hash_hv1',     'HocVien'),
    (N'Trần Nhật Hoàng',    'hoang@student.vn',  'hash_hv2',     'HocVien'),
    (N'Nguyễn Đăng Thắng',  'thang@student.vn',  'hash_hv3',     'HocVien');
GO

-- Huấn luyện viên mẫu
INSERT INTO HuanLuyenVien (MaTaiKhoan, SoDienThoai, ChuyenMon, NgayVaoCLB) VALUES
    (2, '0901234567', N'Quyền pháp', '2022-01-15'),
    (3, '0912345678', N'Vũ khí', '2022-06-01');
GO

-- Học viên mẫu (MaCapDaiHienTai sẽ cập nhật sau khi có CapDai)
INSERT INTO HocVien (MaTaiKhoan, SoDienThoai, NgaySinh, GioiTinh, NgayGiaNhap, MaCapDaiHienTai) VALUES
    (4, '0923456789', '2003-05-15', 'Nam', '2024-01-10', 1),  -- Đai Trắng
    (5, '0934567890', '2004-08-22', 'Nam', '2024-01-10', 2),  -- Đai Vàng
    (6, '0945678901', '2003-12-01', 'Nam', '2024-01-10', 1);  -- Đai Trắng
GO

-- Khóa học mẫu
INSERT INTO KhoaHoc (TenKhoaHoc, NgayKhaiGiang, NgayKetThuc, HocPhi, SoLuongToiDa, TrangThai) VALUES
    (N'Khóa 5 – 2026', '2026-03-01', '2026-05-31', 1200000, 60, 'DangHoc'),
    (N'Khóa 6 – 2026', '2026-06-01', '2026-08-31', 1200000, 60, 'DangTuyenSinh');
GO

-- Lớp học mẫu trong Khóa 5
INSERT INTO LopHoc (MaKhoaHoc, MaCapDai, MaHLV, LichHoc, SoLuongToiDa, PhongTap) VALUES
    (1, 1, 1, N'Thứ 2,4,6 – 17:00-18:30', 25, N'Phòng A'),  -- Lớp Đai Trắng
    (1, 2, 2, N'Thứ 3,5,7 – 17:00-18:30', 20, N'Phòng B');  -- Lớp Đai Vàng
GO

-- Đăng ký lớp mẫu
INSERT INTO DangKyLop (MaHocVien, MaLop, NgayDangKy, TrangThaiThanhToan, NgayThanhToan) VALUES
    (1, 1, '2026-02-28', 'DaThanhToan', '2026-02-28 09:00:00'),
    (3, 1, '2026-02-28', 'DaThanhToan', '2026-02-28 10:00:00'),
    (2, 2, '2026-02-28', 'DaThanhToan', '2026-02-28 11:00:00');
GO

-- Đăng ký kèm mẫu  [v2: MaHLV=NULL, có TrangThaiPhanCong]
INSERT INTO GoiKemRieng (TenGoi, SoBuoi, HocPhi, MaKhoaHoc) VALUES
    (N'Gói kèm 8 buổi', 8, 800000, 1),
    (N'Gói kèm 12 buổi', 12, 1100000, 1);
GO

INSERT INTO DangKyKem (MaHocVien, MaGoi, MaHLV, TrangThaiPhanCong, NgayBatDau, TrangThaiThanhToan) VALUES
    (1, 1, NULL, 'ChoPhanCong', NULL, 'ChuaThanhToan'),  -- HV đăng ký, chờ Admin phân công
    (2, 2, 1,    'DaPhanCong',  '2026-03-10', 'DaThanhToan');  -- Đã phân công HLV Tuấn
GO

-- Điểm danh mẫu
INSERT INTO DiemDanh (MaDangKy, NgayHoc, TrangThai) VALUES
    (1, '2026-03-03', N'CóMặt'),
    (1, '2026-03-05', N'VắngPhép'),
    (2, '2026-03-03', N'CóMặt'),
    (3, '2026-03-04', N'CóMặt');
GO

-- Kỳ thi mẫu
INSERT INTO KyThiThangDai (MaKhoaHoc, NgayThi, MoTa, TrangThai) VALUES
    (1, '2026-05-25', N'Kỳ thi thăng đai cuối Khóa 5', 'SapDienRa');
GO


-- ============================================================
-- CÁC VIEW HỮU ÍCH (gợi ý dùng trong code C#)
-- ============================================================

-- View: Danh sách học viên kèm thông tin đai hiện tại
IF OBJECT_ID('V_HocVienVaCapDai', 'V') IS NOT NULL DROP VIEW V_HocVienVaCapDai;
GO
CREATE VIEW V_HocVienVaCapDai AS
    SELECT
        hv.MaHocVien,
        tk.HoTen,
        tk.Email,
        hv.SoDienThoai,
        hv.NgaySinh,
        hv.GioiTinh,
        cd.TenCapDai,
        cd.MauSac,
        cd.ThuTu AS ThuTuDai,
        hv.NgayGiaNhap
    FROM HocVien hv
    JOIN TaiKhoan tk ON tk.MaTaiKhoan = hv.MaTaiKhoan
    LEFT JOIN CapDai cd ON cd.MaCapDai = hv.MaCapDaiHienTai
    WHERE tk.DangHoatDong = 1;
GO

-- View: Danh sách lớp học kèm tên khóa, cấp đai, HLV
IF OBJECT_ID('V_LopHocChiTiet', 'V') IS NOT NULL DROP VIEW V_LopHocChiTiet;
GO
CREATE VIEW V_LopHocChiTiet AS
    SELECT
        lh.MaLop,
        kh.TenKhoaHoc,
        cd.TenCapDai,
        tk.HoTen AS TenHLV,
        lh.LichHoc,
        lh.SoLuongToiDa,
        lh.PhongTap,
        kh.TrangThai AS TrangThaiKhoa
    FROM LopHoc lh
    JOIN KhoaHoc kh      ON kh.MaKhoaHoc = lh.MaKhoaHoc
    JOIN CapDai cd       ON cd.MaCapDai  = lh.MaCapDai
    JOIN HuanLuyenVien h ON h.MaHLV      = lh.MaHLV
    JOIN TaiKhoan tk     ON tk.MaTaiKhoan = h.MaTaiKhoan;
GO

-- View: Học viên đủ điều kiện thi thăng đai
-- (đã học đủ số tháng tối thiểu ở cấp đai hiện tại)
IF OBJECT_ID('V_HocVienDuDieuKienThi', 'V') IS NOT NULL DROP VIEW V_HocVienDuDieuKienThi;
GO
CREATE VIEW V_HocVienDuDieuKienThi AS
    SELECT
        hv.MaHocVien,
        tk.HoTen,
        cd.TenCapDai AS CapDaiHienTai,
        cd.ThuTu     AS ThuTuDaiHienTai,
        DATEDIFF(MONTH,
            ISNULL((
                SELECT MAX(kythi.NgayThi)
				FROM KetQuaThi kq
				JOIN KyThiThangDai kythi ON kythi.MaKyThi = kq.MaKyThi
				WHERE kq.MaHocVien = hv.MaHocVien AND kq.DaDat = 1
            ), hv.NgayGiaNhap),
            GETDATE()
        ) AS SoThangHienTai,
        cd.ThoiGianToiThieuThang
    FROM HocVien hv
    JOIN TaiKhoan tk ON tk.MaTaiKhoan = hv.MaTaiKhoan
    JOIN CapDai cd   ON cd.MaCapDai   = hv.MaCapDaiHienTai
    WHERE tk.DangHoatDong = 1
      AND cd.ThuTu < 7;  -- Chưa đạt đai Đen (đai cao nhất)
GO

-- View: Danh sách đăng ký kèm chờ phân công  [v2 – mới thêm]
-- Admin dùng view này để biết ai đang chờ được gán HLV
IF OBJECT_ID('V_DangKyKemChoPhanCong', 'V') IS NOT NULL DROP VIEW V_DangKyKemChoPhanCong;
GO
CREATE VIEW V_DangKyKemChoPhanCong AS
    SELECT
        dk.MaDangKyKem,
        tk.HoTen       AS TenHocVien,
        hv.SoDienThoai,
        gk.TenGoi,
        gk.SoBuoi,
        gk.HocPhi,
        kh.TenKhoaHoc,
        dk.NgayDangKy  -- lấy từ cột mặc định GETDATE() lúc INSERT
    FROM DangKyKem dk
    JOIN HocVien hv       ON hv.MaHocVien = dk.MaHocVien
    JOIN TaiKhoan tk      ON tk.MaTaiKhoan = hv.MaTaiKhoan
    JOIN GoiKemRieng gk   ON gk.MaGoi     = dk.MaGoi
    JOIN KhoaHoc kh       ON kh.MaKhoaHoc = gk.MaKhoaHoc
    WHERE dk.TrangThaiPhanCong = 'ChoPhanCong';
GO