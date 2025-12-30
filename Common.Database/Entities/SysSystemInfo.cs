using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class SysSystemInfo
{
    public string Ctyma { get; set; } = null!;

    public string? Ctyten { get; set; }

    public string? CtydiaChi { get; set; }

    public string? CtymaSoThue { get; set; }

    public string? CtydienThoai { get; set; }

    public string? Ctyfax { get; set; }

    public string? Ctyemail { get; set; }

    public string? CtysoTaiKhoan { get; set; }

    public string? CtytenNganHang { get; set; }

    public string? VersionApp { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
