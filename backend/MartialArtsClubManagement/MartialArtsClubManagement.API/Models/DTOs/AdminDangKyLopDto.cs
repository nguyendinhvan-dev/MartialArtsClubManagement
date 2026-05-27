using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminDangKyLopDto
    {
        public int MaDangKy { get; set; }
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; } // From HocVien -> TaiKhoan.HoTen
        public int MaLop { get; set; }
        public string? TenLop { get; set; } // From LopHoc.LichHoc or KhoaHoc.TenKhoaHoc
        public DateOnly NgayDangKy { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class CreateAdminDangKyLopDto
    {
        public int MaHocVien { get; set; }
        public int MaLop { get; set; }
        public DateOnly NgayDangKy { get; set; }
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán";
    }

    public class UpdateAdminDangKyLopDto
    {
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class AdminDiemDanhDto
    {
        public int MaDiemDanh { get; set; }
        public int MaDangKy { get; set; }
        public string? TenHocVien { get; set; }
        public DateOnly NgayHoc { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
    }

    public class CreateAdminDiemDanhDto
    {
        public int MaDangKy { get; set; }
        public DateOnly NgayHoc { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
    }
}
