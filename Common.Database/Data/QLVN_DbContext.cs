using System;
using System.Collections.Generic;
using Common.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Database.Data;

public partial class QLVN_DbContext : DbContext
{
    public QLVN_DbContext()
    {
    }

    public QLVN_DbContext(DbContextOptions<QLVN_DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DbAoNuoi> DbAoNuois { get; set; }

    public virtual DbSet<DbAoNuoiNhapCaHao> DbAoNuoiNhapCaHaos { get; set; }

    public virtual DbSet<DbAoNuoiNhapHc> DbAoNuoiNhapHcs { get; set; }

    public virtual DbSet<DbAoNuoiNhapKc> DbAoNuoiNhapKcs { get; set; }

    public virtual DbSet<DbAoNuoiNhapKhac> DbAoNuoiNhapKhacs { get; set; }

    public virtual DbSet<DbAoNuoiNhapMt> DbAoNuoiNhapMts { get; set; }

    public virtual DbSet<DbAoNuoiNhapSl> DbAoNuoiNhapSls { get; set; }

    public virtual DbSet<DbAoNuoiNhapTg> DbAoNuoiNhapTgs { get; set; }

    public virtual DbSet<DbAoNuoiNhapTh> DbAoNuoiNhapThs { get; set; }

    public virtual DbSet<DbAoNuoiNhapTum> DbAoNuoiNhapTa { get; set; }

    public virtual DbSet<DbDvsd> DbDvsds { get; set; }

    public virtual DbSet<DbHoaChat> DbHoaChats { get; set; }

    public virtual DbSet<DbKhachHang> DbKhachHangs { get; set; }

    public virtual DbSet<DbKhangSinhKq> DbKhangSinhKqs { get; set; }

    public virtual DbSet<DbKhangSinhYc> DbKhangSinhYcs { get; set; }

    public virtual DbSet<DbKhuVuc> DbKhuVucs { get; set; }

    public virtual DbSet<DbLoaiBenh> DbLoaiBenhs { get; set; }

    public virtual DbSet<DbLoaiCa> DbLoaiCas { get; set; }

    public virtual DbSet<DbNhaCungCap> DbNhaCungCaps { get; set; }

    public virtual DbSet<DbNhapTum> DbNhapTa { get; set; }

    public virtual DbSet<DbSizeNl> DbSizeNls { get; set; }

    public virtual DbSet<DbTangTrong> DbTangTrongs { get; set; }

    public virtual DbSet<SysIdGenerated> SysIdGenerateds { get; set; }

    public virtual DbSet<SysMenu> SysMenus { get; set; }

    public virtual DbSet<SysSetting> SysSettings { get; set; }

    public virtual DbSet<SysSystemInfo> SysSystemInfos { get; set; }

    public virtual DbSet<UsGridLayout> UsGridLayouts { get; set; }

    public virtual DbSet<UsGroup> UsGroups { get; set; }

    public virtual DbSet<UsUser> UsUsers { get; set; }

    public virtual DbSet<UsUserLog> UsUserLogs { get; set; }

    public virtual DbSet<UsUserPermission> UsUserPermissions { get; set; }

    public virtual DbSet<VUserLog> VUserLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Chỉ configure nếu chưa được configure (để DI có thể override)
        //if (!optionsBuilder.IsConfigured)
        //{
        //    // Connection string mặc định (chỉ dùng khi không có DI)
        //    optionsBuilder.UseSqlServer(
        //        "Data Source=172.16.80.242,1455;Initial Catalog=IDI_QLVN;Persist Security Info=True;User ID=sa;Password=0303141296;Trust Server Certificate=True",
        //        sqlOptions => sqlOptions.EnableRetryOnFailure(
        //            maxRetryCount: 5,
        //            maxRetryDelay: TimeSpan.FromSeconds(30),
        //            errorNumbersToAdd: null
        //        )
        //    );
        //}
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbAoNuoi>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbAoNuoi_1");

            entity.ToTable("DbAoNuoi");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CongXuatNuoi).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.DienTich).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.GiaGiaCong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.KhachHangMa).HasMaxLength(50);
            entity.Property(e => e.KhuVucMa).HasMaxLength(5);
            entity.Property(e => e.MaSo).HasMaxLength(50);
            entity.Property(e => e.NgayHd)
                .HasMaxLength(50)
                .HasColumnName("NgayHD");
            entity.Property(e => e.NhanVienGsma)
                .HasMaxLength(50)
                .HasColumnName("NhanVienGSMa");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.SlduKien)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("SLDuKien");
            entity.Property(e => e.SoHd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("SoHD");
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapCaHao>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbAoNuoi_NhapCaHao");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.SoCon).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.SoKg).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapCaHaoCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapCaHao_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapCaHaoUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapCaHao_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapHc>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbAoNuoi_NhapHC");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.HoaChatMa)
                .HasMaxLength(5)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.MaSoLo).HasMaxLength(10);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.SoLuong).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapHcCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapHC_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapHcUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapHC_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapKc>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbThongTinKiemCap");

            entity.ToTable("DbAoNuoi_NhapKC");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.LuongGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.LuongThucAn).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.MauGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapKcCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapKC_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapKcUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapKC_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapKhac>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbAoNuoi_NhapKhac");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CaBenhMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.ThayNuoc).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Ttbq)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TTBQ");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapKhacCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapKhac_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapKhacUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapKhac_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapMt>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_Db_AoNuoi_NhapMT");

            entity.ToTable("DbAoNuoi_NhapMT");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.H2s)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("H2S");
            entity.Property(e => e.Kh)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("KH");
            entity.Property(e => e.Nh3)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("NH3");
            entity.Property(e => e.NhietDo).HasColumnType("numeric(18, 1)");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.O2Chieu)
                .HasColumnType("numeric(18, 1)")
                .HasColumnName("O2_Chieu");
            entity.Property(e => e.O2Sang)
                .HasColumnType("numeric(18, 1)")
                .HasColumnName("O2_Sang");
            entity.Property(e => e.PhChieu)
                .HasColumnType("numeric(18, 1)")
                .HasColumnName("PH_Chieu");
            entity.Property(e => e.PhSang)
                .HasColumnType("numeric(18, 1)")
                .HasColumnName("PH_Sang");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapMtCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapMT_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapMtUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapMT_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapSl>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbAoNuoi_SanLuong");

            entity.ToTable("DbAoNuoi_NhapSL");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.LuongGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapSlCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapSL_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapSlUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapSL_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapTg>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbAoNuoi_ThaGiong");

            entity.ToTable("DbAoNuoi_NhapTG");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.GiaGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.LuongGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.MauGiong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapTgCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTG_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapTgUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTG_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapTh>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbAoNuoi_ThuHoach");

            entity.ToTable("DbAoNuoi_NhapTH");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.SanLuong).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Size).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Ta2mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_2mm");
            entity.Property(e => e.Ta3mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_3mm");
            entity.Property(e => e.Ta5mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_5mm");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapThCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTH_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapThUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTH_UsUser1");
        });

        modelBuilder.Entity<DbAoNuoiNhapTum>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbAoNuoi_NhapTA");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.MaLo).HasMaxLength(10);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ta2mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_2mm");
            entity.Property(e => e.Ta3mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_3mm");
            entity.Property(e => e.Ta5mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_5mm");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbAoNuoiNhapTumCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTA_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbAoNuoiNhapTumUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbAoNuoi_NhapTA_UsUser1");
        });

        modelBuilder.Entity<DbDvsd>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbDvsd");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.Cccd)
                .HasMaxLength(50)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.Mst)
                .HasMaxLength(500)
                .HasColumnName("MST");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Stk)
                .HasMaxLength(50)
                .HasColumnName("STK");
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbDvsdCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbDvsd_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbDvsdUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbDvsd_UsUser1");
        });

        modelBuilder.Entity<DbHoaChat>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbHoaChat");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbHoaChatCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbHoaChat_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbHoaChatUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbHoaChat_UsUser1");
        });

        modelBuilder.Entity<DbKhachHang>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbKhachHang_1");

            entity.ToTable("DbKhachHang");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.Cccd)
                .HasMaxLength(50)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Stk)
                .HasMaxLength(50)
                .HasColumnName("STK");
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.TenNganHang).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbKhachHangCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhachHang_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbKhachHangUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhachHang_UsUser1");
        });

        modelBuilder.Entity<DbKhangSinhKq>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbKiemKhangSinh");

            entity.ToTable("DbKhangSinh_KQ");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.Ahd)
                .HasMaxLength(10)
                .HasColumnName("AHD");
            entity.Property(e => e.Amoz)
                .HasMaxLength(10)
                .HasColumnName("AMOZ");
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.Aoz)
                .HasMaxLength(10)
                .HasColumnName("AOZ");
            entity.Property(e => e.Cap)
                .HasMaxLength(10)
                .HasColumnName("CAP");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Enro)
                .HasMaxLength(10)
                .HasColumnName("ENRO");
            entity.Property(e => e.Flu)
                .HasMaxLength(10)
                .HasColumnName("FLU");
            entity.Property(e => e.MaKhangSinh).HasMaxLength(50);
            entity.Property(e => e.Mglmg)
                .HasMaxLength(10)
                .HasColumnName("MGLMG");
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Sem)
                .HasMaxLength(10)
                .HasColumnName("SEM");
            entity.Property(e => e.TenNguoiKiem).HasMaxLength(200);
            entity.Property(e => e.Trf)
                .HasMaxLength(10)
                .HasColumnName("TRF");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbKhangSinhKqCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhangSinh_KQ_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbKhangSinhKqUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhangSinh_KQ_UsUser1");
        });

        modelBuilder.Entity<DbKhangSinhYc>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbAoNuoi_KhangSing_YC");

            entity.ToTable("DbKhangSinh_YC");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.Ahd).HasColumnName("AHD");
            entity.Property(e => e.Amoz).HasColumnName("AMOZ");
            entity.Property(e => e.AoNuoiMa).HasMaxLength(5);
            entity.Property(e => e.Aoz).HasColumnName("AOZ");
            entity.Property(e => e.Cap).HasColumnName("CAP");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Enro).HasColumnName("ENRO");
            entity.Property(e => e.Flu).HasColumnName("FLU");
            entity.Property(e => e.Mglmg).HasColumnName("MGLMG");
            entity.Property(e => e.Sem).HasColumnName("SEM");
            entity.Property(e => e.Trf).HasColumnName("TRF");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbKhangSinhYcCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhangSinh_YC_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbKhangSinhYcUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhangSinh_YC_UsUser1");
        });

        modelBuilder.Entity<DbKhuVuc>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbKhuVuc");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.TinhThanh).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbKhuVucCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhuVuc_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbKhuVucUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbKhuVuc_UsUser1");
        });

        modelBuilder.Entity<DbLoaiBenh>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbLoaiBenh_1");

            entity.ToTable("DbLoaiBenh");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbLoaiBenhCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbLoaiBenh_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbLoaiBenhUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbLoaiBenh_UsUser1");
        });

        modelBuilder.Entity<DbLoaiCa>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbLoaiCa");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbLoaiCaCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbLoaiCa_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbLoaiCaUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbLoaiCa_UsUser1");
        });

        modelBuilder.Entity<DbNhaCungCap>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbNhaCungCap");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.Cccd)
                .HasMaxLength(50)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Stk)
                .HasMaxLength(50)
                .HasColumnName("STK");
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.TenNganHang).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbNhaCungCapCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbNhaCungCap_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbNhaCungCapUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbNhaCungCap_UsUser1");
        });

        modelBuilder.Entity<DbNhapTum>(entity =>
        {
            entity.HasKey(e => e.Ma);

            entity.ToTable("DbNhapTA");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.KhucVucMa).HasMaxLength(5);
            entity.Property(e => e.NhaCungCapMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ta2mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_2mm");
            entity.Property(e => e.Ta3mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_3mm");
            entity.Property(e => e.Ta5mm)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("TA_5mm");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbNhapTumCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbNhapTA_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbNhapTumUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbNhapTA_UsUser1");
        });

        modelBuilder.Entity<DbSizeNl>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbSizeNL_1");

            entity.ToTable("DbSizeNL");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbSizeNlCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbSizeNL_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbSizeNlUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbSizeNL_UsUser1");
        });

        modelBuilder.Entity<DbTangTrong>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK_DbTangTrong_1");

            entity.ToTable("DbTangTrong");

            entity.Property(e => e.Ma).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.DvsdMa).HasMaxLength(5);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.DbTangTrongCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbTangTrong_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.DbTangTrongUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DbTangTrong_UsUser1");
        });

        modelBuilder.Entity<SysIdGenerated>(entity =>
        {
            entity.HasKey(e => e.Table);

            entity.ToTable("SysIdGenerated");

            entity.Property(e => e.Table).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);
        });

        modelBuilder.Entity<SysMenu>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__SysMenu");

            entity.ToTable("SysMenu");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.ParentMenu).HasMaxLength(50);
        });

        modelBuilder.Entity<SysSetting>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.ToTable("SysSetting");

            entity.Property(e => e.Key).HasMaxLength(30);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);
            entity.Property(e => e.Value).HasMaxLength(500);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SysSettings)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SysSetting_UsUserUpdated");
        });

        modelBuilder.Entity<SysSystemInfo>(entity =>
        {
            entity.HasKey(e => e.Ctyma);

            entity.ToTable("SysSystemInfo");

            entity.Property(e => e.Ctyma)
                .HasMaxLength(5)
                .HasColumnName("CTYMa");
            entity.Property(e => e.CtydiaChi)
                .HasMaxLength(500)
                .HasColumnName("CTYDiaChi");
            entity.Property(e => e.CtydienThoai)
                .HasMaxLength(30)
                .HasColumnName("CTYDienThoai");
            entity.Property(e => e.Ctyemail)
                .HasMaxLength(30)
                .HasColumnName("CTYEmail");
            entity.Property(e => e.Ctyfax)
                .HasMaxLength(50)
                .HasColumnName("CTYFax");
            entity.Property(e => e.CtymaSoThue)
                .HasMaxLength(20)
                .HasColumnName("CTYMaSoThue");
            entity.Property(e => e.CtysoTaiKhoan)
                .HasMaxLength(20)
                .HasColumnName("CTYSoTaiKhoan");
            entity.Property(e => e.Ctyten)
                .HasMaxLength(200)
                .HasColumnName("CTYTen");
            entity.Property(e => e.CtytenNganHang)
                .HasMaxLength(100)
                .HasColumnName("CTYTenNganHang");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);
            entity.Property(e => e.VersionApp).HasMaxLength(10);

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SysSystemInfos)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SysSystemInfo_UsUserUpdated");
        });

        modelBuilder.Entity<UsGridLayout>(entity =>
        {
            entity.ToTable("UsGridLayout");

            entity.Property(e => e.Id).HasMaxLength(300);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(8);

            entity.HasOne(d => d.User).WithMany(p => p.UsGridLayouts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsGridLayout_UsUser");
        });

        modelBuilder.Entity<UsGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsGroup");

            entity.ToTable("UsGroup");

            entity.Property(e => e.Id).HasMaxLength(3);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UsGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_UsGroup_UsUser");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UsGroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_UsGroup_UsUser1");
        });

        modelBuilder.Entity<UsUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsUser");

            entity.ToTable("UsUser");

            entity.Property(e => e.Id).HasMaxLength(8);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.CMND)
                .HasMaxLength(20)
                .HasColumnName("CMND");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(8);
            entity.Property(e => e.Email).HasMaxLength(30);
            entity.Property(e => e.GroupId).HasMaxLength(3);
            entity.Property(e => e.Image).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);
            entity.Property(e => e.UserName).HasMaxLength(10);

            entity.HasOne(d => d.Group).WithMany(p => p.UsUsers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsUser_UsGroup");
        });

        modelBuilder.Entity<UsUserLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsUserLog");

            entity.ToTable("UsUserLog");

            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.ComputerName).HasMaxLength(50);
            entity.Property(e => e.Data).HasMaxLength(3000);
            entity.Property(e => e.Menu).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.Property(e => e.UserId).HasMaxLength(8);

            entity.HasOne(d => d.User).WithMany(p => p.UsUserLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsUserLog_UsUser");
        });

        modelBuilder.Entity<UsUserPermission>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MenuId }).HasName("PK__UsUserPe__0B11216FC774DC3E");

            entity.ToTable("UsUserPermission");

            entity.Property(e => e.UserId).HasMaxLength(8);
            entity.Property(e => e.MenuId).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(8);

            entity.HasOne(d => d.Menu).WithMany(p => p.UsUserPermissions)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsUserPermission_SysMenu");

            entity.HasOne(d => d.User).WithMany(p => p.UsUserPermissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsUserPermission_UsUser");
        });

        modelBuilder.Entity<VUserLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vUserLog");

            entity.Property(e => e.ActionDate).HasColumnType("datetime");
            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.ComputerName).HasMaxLength(50);
            entity.Property(e => e.Data).HasMaxLength(3000);
            entity.Property(e => e.GroupId).HasMaxLength(3);
            entity.Property(e => e.GroupName).HasMaxLength(100);
            entity.Property(e => e.Id).HasMaxLength(30);
            entity.Property(e => e.Menu).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.Property(e => e.TenNv)
                .HasMaxLength(100)
                .HasColumnName("TenNV");
            entity.Property(e => e.UserId).HasMaxLength(8);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
