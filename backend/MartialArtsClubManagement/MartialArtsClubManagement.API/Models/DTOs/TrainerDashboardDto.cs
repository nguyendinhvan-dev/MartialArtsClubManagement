using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class TrainerDashboardDto
    {
        public int SoLopDangDay { get; set; }
        public int TongHocVien { get; set; }
        public double TyLeChuyenCan { get; set; }
        public int KyThiSapToi { get; set; }
    }

    public class TrainerLopHocDto
    {
        public int MaLop { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public string? TenCapDai { get; set; }
        public string LichHoc { get; set; } = null!;
        public string? PhongTap { get; set; }
        public int SoLuongToiDa { get; set; }
        public int SoHocVienHienTai { get; set; }
    }

    public class TrainerHocVienDto
    {
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public string? TenCapDai { get; set; }
        public int? MaLop { get; set; }
        public string? TenLop { get; set; }
    }

    public class TrainerDiemDanhDto
    {
        public int MaDiemDanh { get; set; }
        public int MaDangKy { get; set; }
        public string? TenHocVien { get; set; }
        public DateOnly NgayHoc { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? GhiChu { get; set; }
    }

    public class TrainerKyThiDto
    {
        public int MaKyThi { get; set; }
        public string TenKyThi { get; set; } = null!;
        public DateOnly NgayThi { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? MoTa { get; set; }
    }

    public class TrainerKetQuaThiDto
    {
        public int MaKetQua { get; set; }
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public int MaKyThi { get; set; }
        public int? MaCapDaiMoi { get; set; }
        public string? TenCapDaiMoi { get; set; }
        public decimal? DiemSo { get; set; }
    }

    public class TrainerThongBaoDto
    {
        public int MaThongBao { get; set; }
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public DateOnly NgayDang { get; set; }
        public string LoaiThongBao { get; set; } = null!;
    }

    public class TrainerProfileDto
    {
        public int MaHlv { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
        public DateOnly NgayVaoClb { get; set; }
        public bool DangHoatDong { get; set; }
    }

    // Create DTOs
    public class CreateTrainerLopHocDto
    {
        public int MaKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public string LichHoc { get; set; } = null!;
        public int SoLuongToiDa { get; set; }
        public string? PhongTap { get; set; }
    }

    public class UpdateTrainerLopHocDto
    {
        public int MaKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public string LichHoc { get; set; } = null!;
        public int SoLuongToiDa { get; set; }
        public string? PhongTap { get; set; }
    }

    public class CreateTrainerKyThiDto
    {
        public string TenKyThi { get; set; } = null!;
        public DateOnly NgayThi { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? MoTa { get; set; }
    }

    public class UpdateTrainerKyThiDto
    {
        public string TenKyThi { get; set; } = null!;
        public DateOnly NgayThi { get; set; }
        public int MaKhoaHoc { get; set; }
        public string TrangThai { get; set; } = null!;
        public string? MoTa { get; set; }
    }

    public class CreateTrainerThongBaoDto
    {
        public string TieuDe { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public string LoaiThongBao { get; set; } = "ThongBao";
    }

    public class UpdateTrainerProfileDto
    {
        public string? SoDienThoai { get; set; }
        public string? ChuyenMon { get; set; }
    }

    // Bulk attendance DTO
    public class BulkTrainerDiemDanhDto
    {
        public DateOnly NgayHoc { get; set; }
        public List<TrainerDiemDanhItemDto> Attendances { get; set; } = new();
    }

    public class TrainerDiemDanhItemDto
    {
        public int MaDangKy { get; set; }
        public string TrangThai { get; set; } = "CóMặt";
        public string? GhiChu { get; set; }
    }

    // Exam results DTO
    public class BulkTrainerKetQuaThiDto
    {
        public int MaKyThi { get; set; }
        public List<TrainerKetQuaThiItemDto> Results { get; set; } = new();
    }

    public class TrainerKetQuaThiItemDto
    {
        public int MaHocVien { get; set; }
        public int? MaCapDaiMoi { get; set; }
        public decimal? DiemSo { get; set; }
        public bool DaDat { get; set; }
    }

    public class CreateTrainerHocVienDto
    {
        public string TenHocVien { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public int MaLop { get; set; }
    }

    public class UpdateTrainerHocVienDto
    {
        public string TenHocVien { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public int MaLop { get; set; }
    }

    public class TrainerExamResultDetailDto
    {
        public int MaHocVien { get; set; }
        public string? TenHocVien { get; set; }
        public int? MaCapDaiHienTai { get; set; }
        public string? TenCapDaiHienTai { get; set; }
        public int? MaCapDaiMoi { get; set; }
        public string? TenCapDaiMoi { get; set; }
        public decimal? DiemSo { get; set; }
        public bool? DaDat { get; set; }
    }
}
