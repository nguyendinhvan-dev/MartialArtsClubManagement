using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminLopHocPhiDto
    {
        public int MaDangKy { get; set; }
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public int MaLop { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? LichHoc { get; set; }
        public decimal HocPhi { get; set; }
        public DateOnly NgayDangKy { get; set; }
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class AdminKemHocPhiDto
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
        public string TrangThaiThanhToan { get; set; } = null!;
        public DateTime? NgayThanhToan { get; set; }
    }

    public class UpdateHocPhiStatusDto
    {
        public string TrangThaiThanhToan { get; set; } = "Đã thanh toán";
    }
}
