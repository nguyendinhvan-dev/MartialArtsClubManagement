using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class VDangKyKemChoPhanCong
{
    public int MaDangKyKem { get; set; }

    public string TenHocVien { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string TenGoi { get; set; } = null!;

    public int SoBuoi { get; set; }

    public decimal HocPhi { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public DateTime NgayDangKy { get; set; }
}
