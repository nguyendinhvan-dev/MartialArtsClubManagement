using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class DangKyKem
{
    public int MaDangKyKem { get; set; }

    public int MaHocVien { get; set; }

    public int MaGoi { get; set; }

    public int? MaHlv { get; set; }

    public DateTime NgayDangKy { get; set; }

    public string TrangThaiPhanCong { get; set; } = null!;

    public DateOnly? NgayBatDau { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public DateTime? NgayThanhToan { get; set; }

    public virtual GoiKemRieng MaGoiNavigation { get; set; } = null!;

    public virtual HuanLuyenVien? MaHlvNavigation { get; set; }

    public virtual HocVien MaHocVienNavigation { get; set; } = null!;
}
