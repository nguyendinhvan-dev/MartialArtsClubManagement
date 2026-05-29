using System;
using System.Collections.Generic;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class ThongBao
{
    public int MaThongBao { get; set; }

    public int MaTaiKhoanTao { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string LoaiThongBao { get; set; } = null!;

    public string NguoiNhan { get; set; } = "TatCa"; // TatCa, HocVien, HuanLuyenVien, Lop

    public int? MaLop { get; set; } // Nếu NguoiNhan = "Lop" thì chỉ định lớp cụ thể

    public DateTime NgayDang { get; set; }

    public virtual ICollection<DangKySuKien> DangKySuKiens { get; set; } = new List<DangKySuKien>();

    public virtual TaiKhoan MaTaiKhoanTaoNavigation { get; set; } = null!;
}
