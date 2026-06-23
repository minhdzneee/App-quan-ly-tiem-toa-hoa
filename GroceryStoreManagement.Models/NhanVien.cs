namespace GroceryStoreManagement.Models;

public class NhanVien
{
    public int Id { get; set; }
    public string MaNhanVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? ChucVu { get; set; }
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public int? TaiKhoanId { get; set; }
}
