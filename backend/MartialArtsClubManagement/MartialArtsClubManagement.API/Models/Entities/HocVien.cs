using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class HocVien
{
    public int MaHocVien { get; set; }

    public int MaTaiKhoan { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public DateOnly NgayGiaNhap { get; set; }

    public int? MaCapDaiHienTai { get; set; }

    public virtual ICollection<DangKyKem> DangKyKems { get; set; } = new List<DangKyKem>();

    public virtual ICollection<DangKyLop> DangKyLops { get; set; } = new List<DangKyLop>();

    public virtual ICollection<DangKySuKien> DangKySuKiens { get; set; } = new List<DangKySuKien>();

    public virtual ICollection<KetQuaThi> KetQuaThis { get; set; } = new List<KetQuaThi>();

    public virtual CapDai? MaCapDaiHienTaiNavigation { get; set; }

    public virtual TaiKhoan MaTaiKhoanNavigation { get; set; } = null!;
}
