using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class HuanLuyenVien
{
    public int MaHlv { get; set; }

    public int MaTaiKhoan { get; set; }

    public string? SoDienThoai { get; set; }

    public string? ChuyenMon { get; set; }

    public DateOnly NgayVaoClb { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<DangKyKem> DangKyKems { get; set; } = new List<DangKyKem>();

    public virtual ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public virtual TaiKhoan MaTaiKhoanNavigation { get; set; } = null!;
}
