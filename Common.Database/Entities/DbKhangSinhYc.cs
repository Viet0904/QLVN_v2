using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbKhangSinhYc
{
    public string Ma { get; set; } = null!;

    public string AoNuoiMa { get; set; } = null!;

    public string? NoiDungKiem { get; set; }

    public DateOnly NgayYeuCau { get; set; }

    public bool Aoz { get; set; }

    public bool Cap { get; set; }

    public bool Flu { get; set; }

    public bool Enro { get; set; }

    public bool Mglmg { get; set; }

    public bool Trf { get; set; }

    public bool Amoz { get; set; }

    public bool Ahd { get; set; }

    public bool Sem { get; set; }

    public bool Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser CreatedByNavigation { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
