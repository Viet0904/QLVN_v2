using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class UsGroup
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual UsUser? CreatedByNavigation { get; set; }

    public virtual UsUser? UpdatedByNavigation { get; set; }

    public virtual ICollection<UsUser> UsUsers { get; set; } = new List<UsUser>();
}
