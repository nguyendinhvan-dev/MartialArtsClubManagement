using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class KetQuaThi
{
    public int MaKetQua { get; set; }

    public int MaKyThi { get; set; }

    public int MaHocVien { get; set; }

    public decimal DiemSo { get; set; }

    public bool DaDat { get; set; }

    public int? MaCapDaiMoi { get; set; }

    public virtual CapDai? MaCapDaiMoiNavigation { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;

    public virtual KyThiThangDai MaKyThiNavigation { get; set; } = null!;
}
