using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminTaiKhoanDto
    {
        public int MaTaiKhoan { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
        public bool DangHoatDong { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class CreateAdminTaiKhoanDto
    {
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MatKhau { get; set; } = null!; // Plain password, will be hashed in controller
        public string VaiTro { get; set; } = null!;
        public bool DangHoatDong { get; set; } = true;
    }

    public class UpdateAdminTaiKhoanDto
    {
        public string HoTen { get; set; } = null!;
        public string VaiTro { get; set; } = null!;
        public bool DangHoatDong { get; set; }
    }
}
