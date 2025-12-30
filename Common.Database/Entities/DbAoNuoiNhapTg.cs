using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbAoNuoiNhapTg
{
    public string Ma { get; set; } = null!;

    public string AoNuoiMa { get; set; } = null!;

    public DateOnly NgayThaGiong { get; set; }

    public decimal LuongGiong { get; set; }

    public decimal? MauGiong { get; set; }

    public decimal GiaGiong { get; set; }

    public string? Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser CreatedByNavigation { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
