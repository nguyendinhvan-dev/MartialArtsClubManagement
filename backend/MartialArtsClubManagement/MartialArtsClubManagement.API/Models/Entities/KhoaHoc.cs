using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class KhoaHoc
{
    public int MaKhoaHoc { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public DateOnly NgayKhaiGiang { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public decimal HocPhi { get; set; }

    public int SoLuongToiDa { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<GoiKemRieng> GoiKemRiengs { get; set; } = new List<GoiKemRieng>();

    public virtual ICollection<KyThiThangDai> KyThiThangDais { get; set; } = new List<KyThiThangDai>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
