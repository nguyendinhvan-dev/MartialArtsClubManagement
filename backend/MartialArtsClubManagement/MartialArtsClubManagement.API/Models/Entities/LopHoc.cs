using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class LopHoc
{
    public int MaLop { get; set; }

    public int MaKhoaHoc { get; set; }

    public int MaCapDai { get; set; }

    public int MaHlv { get; set; }

    public string LichHoc { get; set; } = null!;

    public string TenLop { get; set; } = null!;
    public decimal HocPhi { get; set; }

    public int SoLuongToiDa { get; set; }

    public string? PhongTap { get; set; }

    public virtual ICollection<DangKyLop> DangKyLops { get; set; } = new List<DangKyLop>();

    public virtual CapDai MaCapDaiNavigation { get; set; } = null!;

    public virtual HuanLuyenVien MaHlvNavigation { get; set; } = null!;

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
