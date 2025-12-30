using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class VUserLog
{
    public string GroupId { get; set; } = null!;

    public string GroupName { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string TenNv { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string Menu { get; set; } = null!;

    public string ComputerName { get; set; } = null!;

    public string ActionName { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime ActionDate { get; set; }

    public string? Data { get; set; }
}
