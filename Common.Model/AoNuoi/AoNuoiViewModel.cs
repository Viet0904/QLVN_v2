namespace Common.Model.AoNuoi;

public class AoNuoiViewModel
{
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
}