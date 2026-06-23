namespace GroceryStoreManagement.Models;

public class ChiTietPhieuNhap
{
    public int Id { get; set; }
    public int PhieuNhapId { get; set; }
    public int SanPhamId { get; set; }
    public string? MaSanPham { get; set; }
    public string? TenSanPham { get; set; }
    public int SoLuong { get; set; }
    public decimal DonGiaNhap { get; set; }
    public decimal ThanhTien { get; set; }

    public decimal TinhThanhTien()
    {
        ThanhTien = SoLuong * DonGiaNhap;
        return ThanhTien;
    }
}
