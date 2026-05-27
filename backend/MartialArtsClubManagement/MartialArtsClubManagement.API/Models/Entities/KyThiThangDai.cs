using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class KyThiThangDai
{
    public int MaKyThi { get; set; }

    public int MaKhoaHoc { get; set; }

    public DateOnly NgayThi { get; set; }

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<KetQuaThi> KetQuaThis { get; set; } = new List<KetQuaThi>();

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
