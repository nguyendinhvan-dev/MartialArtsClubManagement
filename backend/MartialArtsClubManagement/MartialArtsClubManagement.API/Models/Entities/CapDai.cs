using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class CapDai
{
    public int MaCapDai { get; set; }

    public string TenCapDai { get; set; } = null!;

    public string MauSac { get; set; } = null!;

    public int ThuTu { get; set; }

    public int ThoiGianToiThieuThang { get; set; }

    public virtual ICollection<HocVien> HocViens { get; set; } = new List<HocVien>();

    public virtual ICollection<KetQuaThi> KetQuaThis { get; set; } = new List<KetQuaThi>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
