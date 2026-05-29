using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminHocVienDto
    {
        public int MaHocVien { get; set; }
        public int MaTaiKhoan { get; set; }
        public string? TenHocVien { get; set; } // From TaiKhoan.HoTen
        public string? Email { get; set; } // From TaiKhoan.Email
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public DateOnly NgayGiaNhap { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public string? TenCapDai { get; set; } // From CapDai.TenCapDai
    }

    public class CreateAdminHocVienDto
    {
        public int MaTaiKhoan { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public DateOnly NgayGiaNhap { get; set; }
        public int? MaCapDaiHienTai { get; set; }
    }

    public class UpdateAdminHocVienDto
    {
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public DateOnly NgayGiaNhap { get; set; }
        public int? MaCapDaiHienTai { get; set; }
    }
}
