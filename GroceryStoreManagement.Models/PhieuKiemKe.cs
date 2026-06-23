namespace GroceryStoreManagement.Models;

public class PhieuKiemKe
{
    public int Id { get; set; }
    public string MaPhieuKiemKe { get; set; } = string.Empty;
    public DateTime NgayKiemKe { get; set; } = DateTime.Now;
    public int NhanVienId { get; set; }
    public string? TenNhanVien { get; set; }
    public string? GhiChu { get; set; }
    public List<ChiTietKiemKe> ChiTietKiemKes { get; set; } = new();
}
