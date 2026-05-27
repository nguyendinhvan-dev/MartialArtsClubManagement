using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class VLopHocChiTiet
{
    public int MaLop { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public string TenCapDai { get; set; } = null!;

    public string TenHlv { get; set; } = null!;

    public string LichHoc { get; set; } = null!;

    public int SoLuongToiDa { get; set; }

    public string? PhongTap { get; set; }

    public string TrangThaiKhoa { get; set; } = null!;
}
