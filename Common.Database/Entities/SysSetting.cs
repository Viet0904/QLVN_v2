using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class SysSetting
{
    public string Key { get; set; } = null!;

    public string? Description { get; set; }

    public string Value { get; set; } = null!;

    public int Type { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
