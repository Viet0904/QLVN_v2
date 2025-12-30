using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbNhapTum
{
    public string Ma { get; set; } = null!;

    public string KhucVucMa { get; set; } = null!;

    public string NhaCungCapMa { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    public decimal Ta2mm { get; set; }

    public decimal Ta3mm { get; set; }

    public decimal Ta5mm { get; set; }

    public string? Note { get; set; }

    public string DvsdMa { get; set; } = null!;

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser CreatedByNavigation { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
