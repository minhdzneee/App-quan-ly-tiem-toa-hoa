namespace GroceryStoreManagement.Models;

public class LichSuTonKho
{
    public int Id { get; set; }
    public int SanPhamId { get; set; }
    public string? TenSanPham { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.Now;
    public string LoaiBienDong { get; set; } = string.Empty;
    public int SoLuongThayDoi { get; set; }
    public int SoLuongSauBienDong { get; set; }
    public string? LyDo { get; set; }
}
