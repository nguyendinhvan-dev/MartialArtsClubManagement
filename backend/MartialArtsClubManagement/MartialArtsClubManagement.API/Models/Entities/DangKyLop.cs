using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class DangKyLop
{
    public int MaDangKy { get; set; }

    public int MaHocVien { get; set; }

    public int MaLop { get; set; }

    public DateOnly NgayDangKy { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public DateTime? NgayThanhToan { get; set; }

    public virtual ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual LopHoc MaLopNavigation { get; set; } = null!;
}
