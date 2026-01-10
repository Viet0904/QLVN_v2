namespace Common.Model.KhachHang;

public class KHUpdateModel
{
    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Phone { get; set; }

    public string? Cccd { get; set; }

    public string? TenNganHang { get; set; }

    public string? Stk { get; set; }

    public string? GoogleMap { get; set; }

    public string? Note { get; set; }

    public string DvsdMa { get; set; } = null!;

    public int RowStatus { get; set; }
}