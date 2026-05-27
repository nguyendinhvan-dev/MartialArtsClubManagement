using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class DangKySuKien
{
    public int MaDangKySuKien { get; set; }

    public int MaThongBao { get; set; }

    public int MaHocVien { get; set; }

    public DateTime NgayDangKy { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual ThongBao MaThongBaoNavigation { get; set; } = null!;
}
