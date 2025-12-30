using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbNhaCungCap
{
    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Phone { get; set; }

    public string? Cccd { get; set; }

    public string? TenNganHang { get; set; }

    public string? Stk { get; set; }

    public string? GoogleMap { get; set; }

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
