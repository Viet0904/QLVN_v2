using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class UsUser
{
    public string Id { get; set; } = null!;

    public string GroupId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? Gender { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? CMND { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public string? Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public string? Theme { get; set; }

    public virtual ICollection<DbAoNuoi> DbAoNuoiCreatedByNavigations { get; set; } = new List<DbAoNuoi>();

    public virtual ICollection<DbAoNuoiNhapCaHao> DbAoNuoiNhapCaHaoCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapCaHao>();

    public virtual ICollection<DbAoNuoiNhapCaHao> DbAoNuoiNhapCaHaoUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapCaHao>();

    public virtual ICollection<DbAoNuoiNhapHc> DbAoNuoiNhapHcCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapHc>();

    public virtual ICollection<DbAoNuoiNhapHc> DbAoNuoiNhapHcUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapHc>();

    public virtual ICollection<DbAoNuoiNhapKc> DbAoNuoiNhapKcCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapKc>();

    public virtual ICollection<DbAoNuoiNhapKc> DbAoNuoiNhapKcUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapKc>();

    public virtual ICollection<DbAoNuoiNhapKhac> DbAoNuoiNhapKhacCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapKhac>();

    public virtual ICollection<DbAoNuoiNhapKhac> DbAoNuoiNhapKhacUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapKhac>();

    public virtual ICollection<DbAoNuoiNhapMt> DbAoNuoiNhapMtCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapMt>();

    public virtual ICollection<DbAoNuoiNhapMt> DbAoNuoiNhapMtUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapMt>();

    public virtual ICollection<DbAoNuoiNhapSl> DbAoNuoiNhapSlCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapSl>();

    public virtual ICollection<DbAoNuoiNhapSl> DbAoNuoiNhapSlUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapSl>();

    public virtual ICollection<DbAoNuoiNhapTg> DbAoNuoiNhapTgCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapTg>();

    public virtual ICollection<DbAoNuoiNhapTg> DbAoNuoiNhapTgUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapTg>();

    public virtual ICollection<DbAoNuoiNhapTh> DbAoNuoiNhapThCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapTh>();

    public virtual ICollection<DbAoNuoiNhapTh> DbAoNuoiNhapThUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapTh>();

    public virtual ICollection<DbAoNuoiNhapTum> DbAoNuoiNhapTumCreatedByNavigations { get; set; } = new List<DbAoNuoiNhapTum>();

    public virtual ICollection<DbAoNuoiNhapTum> DbAoNuoiNhapTumUpdatedByNavigations { get; set; } = new List<DbAoNuoiNhapTum>();

    public virtual ICollection<DbAoNuoi> DbAoNuoiUpdatedByNavigations { get; set; } = new List<DbAoNuoi>();

    public virtual ICollection<DbDvsd> DbDvsdCreatedByNavigations { get; set; } = new List<DbDvsd>();

    public virtual ICollection<DbDvsd> DbDvsdUpdatedByNavigations { get; set; } = new List<DbDvsd>();

    public virtual ICollection<DbHoaChat> DbHoaChatCreatedByNavigations { get; set; } = new List<DbHoaChat>();

    public virtual ICollection<DbHoaChat> DbHoaChatUpdatedByNavigations { get; set; } = new List<DbHoaChat>();

    public virtual ICollection<DbKhachHang> DbKhachHangCreatedByNavigations { get; set; } = new List<DbKhachHang>();

    public virtual ICollection<DbKhachHang> DbKhachHangUpdatedByNavigations { get; set; } = new List<DbKhachHang>();

    public virtual ICollection<DbKhangSinhKq> DbKhangSinhKqCreatedByNavigations { get; set; } = new List<DbKhangSinhKq>();

    public virtual ICollection<DbKhangSinhKq> DbKhangSinhKqUpdatedByNavigations { get; set; } = new List<DbKhangSinhKq>();

    public virtual ICollection<DbKhangSinhYc> DbKhangSinhYcCreatedByNavigations { get; set; } = new List<DbKhangSinhYc>();

    public virtual ICollection<DbKhangSinhYc> DbKhangSinhYcUpdatedByNavigations { get; set; } = new List<DbKhangSinhYc>();

    public virtual ICollection<DbKhuVuc> DbKhuVucCreatedByNavigations { get; set; } = new List<DbKhuVuc>();

    public virtual ICollection<DbKhuVuc> DbKhuVucUpdatedByNavigations { get; set; } = new List<DbKhuVuc>();

    public virtual ICollection<DbLoaiBenh> DbLoaiBenhCreatedByNavigations { get; set; } = new List<DbLoaiBenh>();

    public virtual ICollection<DbLoaiBenh> DbLoaiBenhUpdatedByNavigations { get; set; } = new List<DbLoaiBenh>();

    public virtual ICollection<DbLoaiCa> DbLoaiCaCreatedByNavigations { get; set; } = new List<DbLoaiCa>();

    public virtual ICollection<DbLoaiCa> DbLoaiCaUpdatedByNavigations { get; set; } = new List<DbLoaiCa>();

    public virtual ICollection<DbNhaCungCap> DbNhaCungCapCreatedByNavigations { get; set; } = new List<DbNhaCungCap>();

    public virtual ICollection<DbNhaCungCap> DbNhaCungCapUpdatedByNavigations { get; set; } = new List<DbNhaCungCap>();

    public virtual ICollection<DbNhapTum> DbNhapTumCreatedByNavigations { get; set; } = new List<DbNhapTum>();

    public virtual ICollection<DbNhapTum> DbNhapTumUpdatedByNavigations { get; set; } = new List<DbNhapTum>();

    public virtual ICollection<DbSizeNl> DbSizeNlCreatedByNavigations { get; set; } = new List<DbSizeNl>();

    public virtual ICollection<DbSizeNl> DbSizeNlUpdatedByNavigations { get; set; } = new List<DbSizeNl>();

    public virtual ICollection<DbTangTrong> DbTangTrongCreatedByNavigations { get; set; } = new List<DbTangTrong>();

    public virtual ICollection<DbTangTrong> DbTangTrongUpdatedByNavigations { get; set; } = new List<DbTangTrong>();

    public virtual UsGroup Group { get; set; } = null!;

    public virtual ICollection<SysSetting> SysSettings { get; set; } = new List<SysSetting>();

    public virtual ICollection<SysSystemInfo> SysSystemInfos { get; set; } = new List<SysSystemInfo>();

    public virtual ICollection<UsGridLayout> UsGridLayouts { get; set; } = new List<UsGridLayout>();

    public virtual ICollection<UsGroup> UsGroupCreatedByNavigations { get; set; } = new List<UsGroup>();

    public virtual ICollection<UsGroup> UsGroupUpdatedByNavigations { get; set; } = new List<UsGroup>();

    public virtual ICollection<UsUserLog> UsUserLogs { get; set; } = new List<UsUserLog>();

    public virtual ICollection<UsUserPermission> UsUserPermissions { get; set; } = new List<UsUserPermission>();
}
