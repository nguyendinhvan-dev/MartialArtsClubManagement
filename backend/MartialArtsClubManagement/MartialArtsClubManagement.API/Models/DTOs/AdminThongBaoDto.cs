using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminThongBaoDto
    {
        public int MaThongBao { get; set; }
        public int MaTaiKhoanTao { get; set; }
        public string? TenNguoiTao { get; set; } // From TaiKhoan.HoTen
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public string LoaiThongBao { get; set; } = null!;
        public string NguoiNhan { get; set; } = "TatCa"; // TatCa, HocVien, HuanLuyenVien, Lop
        public int? MaLop { get; set; }
        public DateTime NgayDang { get; set; }
    }

    public class CreateAdminThongBaoDto
    {
        public int MaTaiKhoanTao { get; set; }
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public string LoaiThongBao { get; set; } = "Chung";
        public string NguoiNhan { get; set; } = "TatCa";
        public int? MaLop { get; set; }
    }

    public class UpdateAdminThongBaoDto
    {
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public string LoaiThongBao { get; set; } = null!;
        public string NguoiNhan { get; set; } = "TatCa";
        public int? MaLop { get; set; }
    }
}
