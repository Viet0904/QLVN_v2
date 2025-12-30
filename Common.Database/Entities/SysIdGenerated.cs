using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class SysIdGenerated
{
    public string Table { get; set; } = null!;

    public int TotalRows { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;
}
