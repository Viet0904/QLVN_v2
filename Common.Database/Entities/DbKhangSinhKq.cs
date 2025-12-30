using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbKhangSinhKq
{
    public string Ma { get; set; } = null!;

    public string AoNuoiMa { get; set; } = null!;

    public string MaKhangSinh { get; set; } = null!;

    public DateOnly NgayKiem { get; set; }

    public string? Aoz { get; set; }

    public string? Cap { get; set; }

    public string? Flu { get; set; }

    public string? Enro { get; set; }

    public string? Mglmg { get; set; }

    public string? Trf { get; set; }

    public string? Amoz { get; set; }

    public string? Ahd { get; set; }

    public string? Sem { get; set; }

    public string? TenNguoiKiem { get; set; }

    public string? Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser CreatedByNavigation { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
