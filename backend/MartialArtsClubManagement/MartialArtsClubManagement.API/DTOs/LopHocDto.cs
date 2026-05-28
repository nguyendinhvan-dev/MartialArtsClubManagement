using System;

namespace MartialArtsClubManagement.API.DTOs
{
    public class LopHocDto
    {
        public int MaLop { get; set; }
        public string TenLop { get; set; } = string.Empty;
        public int MaKhoaHoc { get; set; }
        public int MaCapDai { get; set; }
        public DateTime LichHoc { get; set; }
        public int SoLuongToiDa { get; set; }
        public string PhongTap { get; set; } = string.Empty;
        public decimal HocPhi { get; set; }
    }
}
