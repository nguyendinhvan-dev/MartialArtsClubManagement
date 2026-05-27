using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminDangKyKemDto
    {
        public int MaDangKyKem { get; set; }
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public int MaGoi { get; set; }
        public string? TenGoi { get; set; }
        public int SoBuoi { get; set; }
        public decimal HocPhi { get; set; }
        public int? MaHlv { get; set; }
        public string? TenHuanLuyenVien { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string TrangThaiPhanCong { get; set; } = null!;
        public DateOnly? NgayBatDau { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class CreateAdminDangKyKemDto
    {
        public int MaHocVien { get; set; }
        public int MaGoi { get; set; }
        public int? MaHlv { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public string TrangThaiPhanCong { get; set; } = "ChoPhanCong";
        public DateOnly? NgayBatDau { get; set; }
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan";
    }

    public class UpdateAdminDangKyKemDto
    {
        public int? MaHlv { get; set; }
        public string TrangThaiPhanCong { get; set; } = null!;
        public DateOnly? NgayBatDau { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class AssignTutorDto
    {
        public int MaHlv { get; set; }
    }

    public class DangKyKemChoPhanCongDto
    {
        public int MaDangKyKem { get; set; }
        public string TenHocVien { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string TenGoi { get; set; } = null!;
        public int SoBuoi { get; set; }
        public decimal HocPhi { get; set; }
        public string TenKhoaHoc { get; set; } = null!;
        public DateTime NgayDangKy { get; set; }
    }
}
