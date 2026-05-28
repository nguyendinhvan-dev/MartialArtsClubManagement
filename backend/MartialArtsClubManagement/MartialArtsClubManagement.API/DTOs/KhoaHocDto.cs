using System;

namespace MartialArtsClubManagement.API.DTOs
{
    public class KhoaHocDto
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public int MaHuongDanVien { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal HocPhi { get; set; }
    }
}
