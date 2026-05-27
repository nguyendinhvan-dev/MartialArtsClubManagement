using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminKhoaHocDto
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = null!;
        public DateOnly NgayKhaiGiang { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public decimal HocPhi { get; set; }
        public int SoLuongToiDa { get; set; }
        public string TrangThai { get; set; } = null!;
    }

    public class CreateAdminKhoaHocDto
    {
        public string TenKhoaHoc { get; set; } = null!;
        public DateOnly NgayKhaiGiang { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public decimal HocPhi { get; set; }
        public int SoLuongToiDa { get; set; }
        public string TrangThai { get; set; } = "Mở đăng ký";
    }

    public class UpdateAdminKhoaHocDto
    {
        public string TenKhoaHoc { get; set; } = null!;
        public DateOnly NgayKhaiGiang { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public decimal HocPhi { get; set; }
        public int SoLuongToiDa { get; set; }
        public string TrangThai { get; set; } = null!;
    }
}
