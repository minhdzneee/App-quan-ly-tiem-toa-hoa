namespace GroceryStoreManagement.Models;

public class ChiTietHoaDon
{
    public int Id { get; set; }
    public int HoaDonId { get; set; }
    public int SanPhamId { get; set; }
    public string? MaSanPham { get; set; }
    public string? TenSanPham { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }

    public decimal TinhThanhTien()
    {
        ThanhTien = SoLuong * DonGia;
        return ThanhTien;
    }
}
