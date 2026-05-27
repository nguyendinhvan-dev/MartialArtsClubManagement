using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public bool DangHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual HocVien? HocVien { get; set; }

    public virtual HuanLuyenVien? HuanLuyenVien { get; set; }

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
