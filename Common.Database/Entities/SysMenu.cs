using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class SysMenu
{
    public string Name { get; set; } = null!;

    public string? ParentMenu { get; set; }

    public string Note { get; set; } = null!;

    public string? Icon { get; set; }

    public int? IsActive { get; set; }

    public virtual ICollection<UsUserPermission> UsUserPermissions { get; set; } = new List<UsUserPermission>();
}
