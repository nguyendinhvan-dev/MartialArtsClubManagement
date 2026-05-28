using System;

namespace MartialArtsClubManagement.API.DTOs
{
    public class DiemDanhDto
    {
        public int MaDiemDanh { get; set; }
        public int MaLop { get; set; }
        public int MaHocVien { get; set; }
        public DateTime NgayDiemDanh { get; set; }
        public bool DaDiemDanh { get; set; }
    }
}
