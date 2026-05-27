using System;

namespace MartialArtsClubManagement.API.Models.DTOs
{
    public class AdminCapDaiDto
    {
        public int MaCapDai { get; set; }
        public string TenCapDai { get; set; } = null!;
        public string MauSac { get; set; } = null!;
        public int ThuTu { get; set; }
        public int ThoiGianToiThieuThang { get; set; }
    }

    public class CreateAdminCapDaiDto
    {
        public string TenCapDai { get; set; } = null!;
        public string MauSac { get; set; } = null!;
        public int ThuTu { get; set; }
        public int ThoiGianToiThieuThang { get; set; }
    }

    public class UpdateAdminCapDaiDto
    {
        public string TenCapDai { get; set; } = null!;
        public string MauSac { get; set; } = null!;
        public int ThuTu { get; set; }
        public int ThoiGianToiThieuThang { get; set; }
    }
}
