using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminKyThiThangDaiDto
    {
        public int MaKyThi { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public DateOnly NgayThi { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = null!;
    }

    public class CreateAdminKyThiThangDaiDto
    {
        public int MaKhoaHoc { get; set; }
        public DateOnly NgayThi { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = "SapDienRa";
    }

    public class UpdateAdminKyThiThangDaiDto
    {
        public int MaKhoaHoc { get; set; }
        public DateOnly NgayThi { get; set; }
        public string? MoTa { get; set; }
        public string TrangThai { get; set; } = null!;
    }

    public class AdminKetQuaThiDto
    {
        public int MaKetQua { get; set; }
        public int MaKyThi { get; set; }
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public decimal DiemSo { get; set; }
        public bool DaDat { get; set; }
        public int? MaCapDaiMoi { get; set; }
        public string? TenCapDaiMoi { get; set; }
    }

    public class CreateAdminKetQuaThiDto
    {
        public int MaHocVien { get; set; }
        public decimal DiemSo { get; set; }
        public bool DaDat { get; set; }
        public int? MaCapDaiMoi { get; set; }
    }

    public class UpdateAdminKetQuaThiDto
    {
        public decimal DiemSo { get; set; }
        public bool DaDat { get; set; }
        public int? MaCapDaiMoi { get; set; }
    }

    public class HocVienDuDieuKienThiDto
    {
        public int MaHocVien { get; set; }
        public string HoTen { get; set; } = null!;
        public string CapDaiHienTai { get; set; } = null!;
        public int ThuTuDaiHienTai { get; set; }
        public int? SoThangHienTai { get; set; }
        public int ThoiGianToiThieuThang { get; set; }
    }

    public class AdminHocVienSimpleDto
    {
        public int MaHocVien { get; set; }
        public string? HoTen { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public string? TenCapDaiHienTai { get; set; }
    }
}
