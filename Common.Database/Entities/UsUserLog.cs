using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class UsUserLog
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Menu { get; set; } = null!;

    public string ComputerName { get; set; } = null!;

    public string ActionName { get; set; } = null!;

    public string? Data { get; set; }

    public string? Note { get; set; }

    public DateTime ActionDate { get; set; }

    public virtual UsUser User { get; set; } = null!;
}
