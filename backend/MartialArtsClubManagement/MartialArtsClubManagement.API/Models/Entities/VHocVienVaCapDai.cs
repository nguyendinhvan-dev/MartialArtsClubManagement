using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class VHocVienVaCapDai
{
    public int MaHocVien { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? TenCapDai { get; set; }

    public string? MauSac { get; set; }

    public int? ThuTuDai { get; set; }

    public DateOnly NgayGiaNhap { get; set; }
}
