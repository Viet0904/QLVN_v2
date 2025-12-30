using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class UsGridLayout
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Layout { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UsUser User { get; set; } = null!;
}
