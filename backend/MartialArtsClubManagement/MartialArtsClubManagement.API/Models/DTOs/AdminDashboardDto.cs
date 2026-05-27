using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminDashboardStatsDto
    {
        public int TongHocVien { get; set; }
        public int TongHuanLuyenVien { get; set; }
        public int TongLopHoc { get; set; }
        public decimal DoanhThuThangNay { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Thang { get; set; }
        public decimal DoanhThuLopHoc { get; set; }
        public decimal DoanhThuKemRieng { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class ClassStudentCountDto
    {
        public int MaLop { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? LichHoc { get; set; }
        public int SoLuongHocVien { get; set; }
        public int SoLuongToiDa { get; set; }
    }

    public class BeltDistributionDto
    {
        public int MaCapDai { get; set; }
        public string? TenCapDai { get; set; }
        public string? MauSac { get; set; }
        public int SoLuongHocVien { get; set; }
    }
}
