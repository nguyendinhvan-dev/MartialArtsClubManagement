using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MartialArtsClubManagement.API.Models.Entities;

public partial class QuanLyCLBVoThuatContext : DbContext
{
    public QuanLyCLBVoThuatContext()
    {
    }

    public QuanLyCLBVoThuatContext(DbContextOptions<QuanLyCLBVoThuatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CapDai> CapDais { get; set; }

    public virtual DbSet<DangKyKem> DangKyKems { get; set; }

    public virtual DbSet<DangKyLop> DangKyLops { get; set; }

    public virtual DbSet<DangKySuKien> DangKySuKiens { get; set; }

    public virtual DbSet<DiemDanh> DiemDanhs { get; set; }

    public virtual DbSet<GoiKemRieng> GoiKemRiengs { get; set; }

    public virtual DbSet<HocVien> HocViens { get; set; }

    public virtual DbSet<HuanLuyenVien> HuanLuyenViens { get; set; }

    public virtual DbSet<KetQuaThi> KetQuaThis { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<KyThiThangDai> KyThiThangDais { get; set; }

    public virtual DbSet<LopHoc> LopHocs { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<VDangKyKemChoPhanCong> VDangKyKemChoPhanCongs { get; set; }

    public virtual DbSet<VHocVienDuDieuKienThi> VHocVienDuDieuKienThis { get; set; }

    public virtual DbSet<VHocVienVaCapDai> VHocVienVaCapDais { get; set; }

    public virtual DbSet<VLopHocChiTiet> VLopHocChiTiets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=VAN;Database=QuanLyCLBVoThuat;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapDai>(entity =>
        {
            entity.HasKey(e => e.MaCapDai).HasName("PK__CapDai__ABF5572A03D91FAD");

            entity.ToTable("CapDai");

            entity.HasIndex(e => e.ThuTu, "UQ__CapDai__2E2833D103247378").IsUnique();

            entity.Property(e => e.MauSac).HasMaxLength(30);
            entity.Property(e => e.TenCapDai).HasMaxLength(50);
            entity.Property(e => e.ThoiGianToiThieuThang).HasDefaultValue(3);
        });

        modelBuilder.Entity<DangKyKem>(entity =>
        {
            entity.HasKey(e => e.MaDangKyKem).HasName("PK__DangKyKe__113944DB23FBCD52");

            entity.ToTable("DangKyKem");

            entity.HasIndex(e => new { e.MaHocVien, e.MaGoi }, "UQ_DangKyKem").IsUnique();

            entity.Property(e => e.MaHlv).HasColumnName("MaHLV");
            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TrangThaiPhanCong)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("ChoPhanCong");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("ChuaThanhToan");

            entity.HasOne(d => d.MaGoiNavigation).WithMany(p => p.DangKyKems)
                .HasForeignKey(d => d.MaGoi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyKem__MaGoi__72C60C4A");

            entity.HasOne(d => d.MaHlvNavigation).WithMany(p => p.DangKyKems)
                .HasForeignKey(d => d.MaHlv)
                .HasConstraintName("FK__DangKyKem__MaHLV__73BA3083");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKyKems)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyKem__MaHoc__71D1E811");
        });

        modelBuilder.Entity<DangKyLop>(entity =>
        {
            entity.HasKey(e => e.MaDangKy).HasName("PK__DangKyLo__BA90F02DA76F7639");

            entity.ToTable("DangKyLop");

            entity.HasIndex(e => new { e.MaHocVien, e.MaLop }, "UQ_DangKyLop").IsUnique();

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("ChuaThanhToan");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKyLops)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyLop__MaHoc__5DCAEF64");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.DangKyLops)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyLop__MaLop__5EBF139D");
        });

        modelBuilder.Entity<DangKySuKien>(entity =>
        {
            entity.HasKey(e => e.MaDangKySuKien).HasName("PK__DangKySu__2308849504373920");

            entity.ToTable("DangKySuKien");

            entity.HasIndex(e => new { e.MaThongBao, e.MaHocVien }, "UQ_DangKySuKien").IsUnique();

            entity.Property(e => e.NgayDangKy)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.DangKySuKiens)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKySuK__MaHoc__0B91BA14");

            entity.HasOne(d => d.MaThongBaoNavigation).WithMany(p => p.DangKySuKiens)
                .HasForeignKey(d => d.MaThongBao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKySuK__MaTho__0A9D95DB");
        });

        modelBuilder.Entity<DiemDanh>(entity =>
        {
            entity.HasKey(e => e.MaDiemDanh).HasName("PK__DiemDanh__1512439D092DD1C7");

            entity.ToTable("DiemDanh");

            entity.HasIndex(e => new { e.MaDangKy, e.NgayHoc }, "UQ_DiemDanh").IsUnique();

            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("CóMặt");

            entity.HasOne(d => d.MaDangKyNavigation).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.MaDangKy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DiemDanh__MaDang__6477ECF3");
        });

        modelBuilder.Entity<GoiKemRieng>(entity =>
        {
            entity.HasKey(e => e.MaGoi).HasName("PK__GoiKemRi__3CD30F69BC1493F8");

            entity.ToTable("GoiKemRieng");

            entity.Property(e => e.DangSuDung).HasDefaultValue(true);
            entity.Property(e => e.HocPhi).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TenGoi).HasMaxLength(100);

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.GoiKemRiengs)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoiKemRie__MaKho__693CA210");
        });

        modelBuilder.Entity<HocVien>(entity =>
        {
            entity.HasKey(e => e.MaHocVien).HasName("PK__HocVien__685B0E6A7E5D40CD");

            entity.ToTable("HocVien");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__HocVien__AD7C6528BDD8D950").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.NgayGiaNhap).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaCapDaiHienTaiNavigation).WithMany(p => p.HocViens)
                .HasForeignKey(d => d.MaCapDaiHienTai)
                .HasConstraintName("FK_HocVien_CapDai");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.HocVien)
                .HasForeignKey<HocVien>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HocVien__MaTaiKh__49C3F6B7");
        });

        modelBuilder.Entity<HuanLuyenVien>(entity =>
        {
            entity.HasKey(e => e.MaHlv).HasName("PK__HuanLuye__3C9029D84A99DD0D");

            entity.ToTable("HuanLuyenVien");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__HuanLuye__AD7C6528A0648C92").IsUnique();

            entity.Property(e => e.MaHlv).HasColumnName("MaHLV");
            entity.Property(e => e.ChuyenMon).HasMaxLength(100);
            entity.Property(e => e.DangHoatDong).HasDefaultValue(true);
            entity.Property(e => e.NgayVaoClb)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("NgayVaoCLB");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.HuanLuyenVien)
                .HasForeignKey<HuanLuyenVien>(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HuanLuyen__MaTai__403A8C7D");
        });

        modelBuilder.Entity<KetQuaThi>(entity =>
        {
            entity.HasKey(e => e.MaKetQua).HasName("PK__KetQuaTh__D5B3102A0631BB40");

            entity.ToTable("KetQuaThi");

            entity.HasIndex(e => new { e.MaKyThi, e.MaHocVien }, "UQ_KetQuaThi").IsUnique();

            entity.Property(e => e.DiemSo).HasColumnType("decimal(4, 1)");

            entity.HasOne(d => d.MaCapDaiMoiNavigation).WithMany(p => p.KetQuaThis)
                .HasForeignKey(d => d.MaCapDaiMoi)
                .HasConstraintName("FK__KetQuaThi__MaCap__00200768");

            entity.HasOne(d => d.MaHocVienNavigation).WithMany(p => p.KetQuaThis)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KetQuaThi__MaHoc__7F2BE32F");

            entity.HasOne(d => d.MaKyThiNavigation).WithMany(p => p.KetQuaThis)
                .HasForeignKey(d => d.MaKyThi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KetQuaThi__MaKyT__7E37BEF6");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.MaKhoaHoc).HasName("PK__KhoaHoc__48F0FF983557B136");

            entity.ToTable("KhoaHoc");

            entity.Property(e => e.HocPhi).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.SoLuongToiDa).HasDefaultValue(100);
            entity.Property(e => e.TenKhoaHoc).HasMaxLength(100);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ChuaMo");
        });

        modelBuilder.Entity<KyThiThangDai>(entity =>
        {
            entity.HasKey(e => e.MaKyThi).HasName("PK__KyThiTha__1403DE9874F6BA18");

            entity.ToTable("KyThiThangDai");

            entity.Property(e => e.MoTa).HasMaxLength(300);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("SapDienRa");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.KyThiThangDais)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KyThiThan__MaKho__787EE5A0");
        });

        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__LopHoc__3B98D2731D2B6812");

            entity.ToTable("LopHoc");

            entity.HasIndex(e => new { e.MaKhoaHoc, e.MaCapDai }, "UQ_LopHoc_KhoaCapDai").IsUnique();

            entity.Property(e => e.LichHoc).HasMaxLength(100);
            entity.Property(e => e.MaHlv).HasColumnName("MaHLV");
            entity.Property(e => e.PhongTap).HasMaxLength(50);
            entity.Property(e => e.SoLuongToiDa).HasDefaultValue(30);

            entity.HasOne(d => d.MaCapDaiNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaCapDai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LopHoc__MaCapDai__5629CD9C");

            entity.HasOne(d => d.MaHlvNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaHlv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LopHoc__MaHLV__571DF1D5");

            entity.HasOne(d => d.MaKhoaHocNavigation).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LopHoc__MaKhoaHo__5535A963");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C652998BD228F");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D105346080821C").IsUnique();

            entity.Property(e => e.DangHoatDong).HasDefaultValue(true);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__ThongBao__04DEB54ECFA13B52");

            entity.ToTable("ThongBao");

            entity.Property(e => e.LoaiThongBao)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("ThongBao");
            entity.Property(e => e.NgayDang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaTaiKhoanTaoNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaTaiKhoanTao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThongBao__MaTaiK__05D8E0BE");
        });

        modelBuilder.Entity<VDangKyKemChoPhanCong>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_DangKyKemChoPhanCong");

            entity.Property(e => e.HocPhi).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.NgayDangKy).HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenGoi).HasMaxLength(100);
            entity.Property(e => e.TenHocVien).HasMaxLength(100);
            entity.Property(e => e.TenKhoaHoc).HasMaxLength(100);
        });

        modelBuilder.Entity<VHocVienDuDieuKienThi>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_HocVienDuDieuKienThi");

            entity.Property(e => e.CapDaiHienTai).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
        });

        modelBuilder.Entity<VHocVienVaCapDai>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_HocVienVaCapDai");

            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MauSac).HasMaxLength(30);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenCapDai).HasMaxLength(50);
        });

        modelBuilder.Entity<VLopHocChiTiet>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_LopHocChiTiet");

            entity.Property(e => e.LichHoc).HasMaxLength(100);
            entity.Property(e => e.PhongTap).HasMaxLength(50);
            entity.Property(e => e.TenCapDai).HasMaxLength(50);
            entity.Property(e => e.TenHlv)
                .HasMaxLength(100)
                .HasColumnName("TenHLV");
            entity.Property(e => e.TenKhoaHoc).HasMaxLength(100);
            entity.Property(e => e.TrangThaiKhoa)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
