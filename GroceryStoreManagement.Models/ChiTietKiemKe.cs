namespace GroceryStoreManagement.Models;

public class ChiTietKiemKe
{
    public int Id { get; set; }
    public int PhieuKiemKeId { get; set; }
    public int SanPhamId { get; set; }
    public string? MaSanPham { get; set; }
    public string? TenSanPham { get; set; }
    public int SoLuongHeThong { get; set; }
    public int SoLuongThucTe { get; set; }
    public int ChenhLech { get; set; }

    public int TinhChenhLech()
    {
        ChenhLech = SoLuongThucTe - SoLuongHeThong;
        return ChenhLech;
    }
}
