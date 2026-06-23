namespace GroceryStoreManagement.Models;

public class KhachHang
{
    public int Id { get; set; }
    public string MaKhachHang { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public int DiemTichLuy { get; set; }
}
