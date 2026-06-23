namespace GroceryStoreManagement.Models;

public class SanPhamBanChay
{
    public int SanPhamId { get; set; }
    public string MaSanPham { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}
