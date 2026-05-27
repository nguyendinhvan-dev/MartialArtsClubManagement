using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class DiemDanh
{
    public int MaDiemDanh { get; set; }

    public int MaDangKy { get; set; }

    public DateOnly NgayHoc { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual DangKyLop MaDangKyNavigation { get; set; } = null!;
}
