using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminLopHocDto
    {
        public int MaLop { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; } // From KhoaHoc.TenKhoaHoc
        public int MaCapDai { get; set; }
        public string? TenCapDai { get; set; } // From CapDai.TenCapDai
        public int MaHlv { get; set; }
        public string? TenHuanLuyenVien { get; set; } // From HuanLuyenVien.MaTaiKhoanNavigation.HoTen
        public string LichHoc { get; set; } = null!;
        public int SoLuongToiDa { get; set; }
        public string? PhongTap { get; set; }
    }

    public class CreateAdminLopHocDto
    {
        public int MaKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public int MaHlv { get; set; }
        public string LichHoc { get; set; } = null!;
        public int SoLuongToiDa { get; set; }
        public string? PhongTap { get; set; }
    }

    public class UpdateAdminLopHocDto
    {
        public int MaKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public int MaHlv { get; set; }
        public string LichHoc { get; set; } = null!;
        public int SoLuongToiDa { get; set; }
        public string? PhongTap { get; set; }
    }
}
