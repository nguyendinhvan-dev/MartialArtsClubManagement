using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class GoiKemRieng
{
    public int MaGoi { get; set; }

    public string TenGoi { get; set; } = null!;

    public int SoBuoi { get; set; }

    public decimal HocPhi { get; set; }

    public int MaKhoaHoc { get; set; }

    public bool DangSuDung { get; set; }

    public virtual ICollection<DangKyKem> DangKyKems { get; set; } = new List<DangKyKem>();

    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
}
