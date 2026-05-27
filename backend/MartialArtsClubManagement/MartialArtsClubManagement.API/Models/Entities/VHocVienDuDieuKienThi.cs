using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class VHocVienDuDieuKienThi
{
    public int MaHocVien { get; set; }

    public string HoTen { get; set; } = null!;

    public string CapDaiHienTai { get; set; } = null!;

    public int ThuTuDaiHienTai { get; set; }

    public int? SoThangHienTai { get; set; }

    public int ThoiGianToiThieuThang { get; set; }
}
