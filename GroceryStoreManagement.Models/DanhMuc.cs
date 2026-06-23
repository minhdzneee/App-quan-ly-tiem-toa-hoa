namespace GroceryStoreManagement.Models;

public class DanhMuc
{
    public int Id { get; set; }
    public string MaDanhMuc { get; set; } = string.Empty;
    public string TenDanhMuc { get; set; } = string.Empty;
    public string? MoTa { get; set; }
}
