namespace GroceryStoreManagement.Models;

public class NhaCungCap
{
    public int Id { get; set; }
    public string MaNhaCungCap { get; set; } = string.Empty;
    public string TenNhaCungCap { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? Email { get; set; }
}
