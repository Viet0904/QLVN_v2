using System;
using System.Collections.Generic;

namespace Common.Database.Entities;

public partial class DbAoNuoi
{
    public string Ma { get; set; } = null!;

    public string DvsdMa { get; set; } = null!;

    public string? MaSo { get; set; }

    public string Ten { get; set; } = null!;

    public DateOnly? NgayCap { get; set; }

    public string? DiaChi { get; set; }

    public decimal DienTich { get; set; }

    public DateOnly? NgayThuHoach { get; set; }

    public decimal SlduKien { get; set; }

    public string? SoHd { get; set; }

    public string? NgayHd { get; set; }

    public decimal GiaGiaCong { get; set; }

    public string? NhanVienGsma { get; set; }

    public decimal CongXuatNuoi { get; set; }

    public bool TinhTrang { get; set; }

    public string KhachHangMa { get; set; } = null!;

    public string KhuVucMa { get; set; } = null!;

    public string? GoogleMap { get; set; }

    public string? Note { get; set; }

    public int RowStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual UsUser CreatedByNavigation { get; set; } = null!;

    public virtual UsUser UpdatedByNavigation { get; set; } = null!;
}
