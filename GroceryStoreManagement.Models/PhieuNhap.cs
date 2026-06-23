namespace GroceryStoreManagement.Models;

public class PhieuNhap
{
    public int Id { get; set; }
    public string MaPhieuNhap { get; set; } = string.Empty;
    public DateTime NgayNhap { get; set; } = DateTime.Now;
    public int NhaCungCapId { get; set; }
    public string? TenNhaCungCap { get; set; }
    public int NhanVienId { get; set; }
    public string? TenNhanVien { get; set; }
    public decimal TongTien { get; set; }
    public string? GhiChu { get; set; }
    public List<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new();

    public void LapPhieuNhap()
    {
        TongTien = TinhTongTien();
    }

    public decimal TinhTongTien()
    {
        return ChiTietPhieuNhaps.Sum(item => item.TinhThanhTien());
    }
}
