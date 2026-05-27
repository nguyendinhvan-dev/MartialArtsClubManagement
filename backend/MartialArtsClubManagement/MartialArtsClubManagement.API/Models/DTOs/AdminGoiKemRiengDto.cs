using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminGoiKemRiengDto
    {
        public int MaGoi { get; set; }
        public string TenGoi { get; set; } = null!;
        public int SoBuoi { get; set; }
        public decimal HocPhi { get; set; }
        public int MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public bool DangSuDung { get; set; }
    }

    public class CreateAdminGoiKemRiengDto
    {
        public string TenGoi { get; set; } = null!;
        public int SoBuoi { get; set; }
        public decimal HocPhi { get; set; }
        public int MaKhoaHoc { get; set; }
        public bool DangSuDung { get; set; } = true;
    }

    public class UpdateAdminGoiKemRiengDto
    {
        public string TenGoi { get; set; } = null!;
        public int SoBuoi { get; set; }
        public decimal HocPhi { get; set; }
        public int MaKhoaHoc { get; set; }
        public bool DangSuDung { get; set; }
    }
}
