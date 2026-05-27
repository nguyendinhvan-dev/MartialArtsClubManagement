using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminHuanLuyenVienDto
    {
        public int MaHlv { get; set; }
        public int MaTaiKhoan { get; set; }
        public string? TenHuanLuyenVien { get; set; } // From TaiKhoan.HoTen
        public string? Email { get; set; } // From TaiKhoan.Email
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
        public DateOnly NgayVaoClb { get; set; }
        public bool DangHoatDong { get; set; }
    }

    public class CreateAdminHuanLuyenVienDto
    {
        public int MaTaiKhoan { get; set; }
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
        public DateOnly NgayVaoClb { get; set; }
        public bool DangHoatDong { get; set; } = true;
    }

    public class UpdateAdminHuanLuyenVienDto
    {
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
        public DateOnly NgayVaoClb { get; set; }
        public bool DangHoatDong { get; set; }
    }
}
