namespace GroceryStoreManagement.Models;

public class HoaDon
{
    public int Id { get; set; }
    public string MaHoaDon { get; set; } = string.Empty;
    public DateTime NgayLap { get; set; } = DateTime.Now;
    public int NhanVienId { get; set; }
    public string? TenNhanVien { get; set; }
    public int? KhachHangId { get; set; }
    public string? TenKhachHang { get; set; }
    public decimal TongTien { get; set; }
    public decimal GiamGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string TrangThai { get; set; } = "DaThanhToan";
    public List<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new();

    public decimal TinhTongTien()
    {
        TongTien = ChiTietHoaDons.Sum(item => item.TinhThanhTien());
        ThanhTien = Math.Max(0, TongTien - GiamGia);
        return TongTien;
    }

    public void ThanhToan(decimal giamGia)
    {
        GiamGia = giamGia;
        TinhTongTien();
        TrangThai = "DaThanhToan";
    }

    public string InHoaDon()
    {
        return $"{MaHoaDon} - {NgayLap:dd/MM/yyyy HH:mm} - {ThanhTien:n0}";
    }
}
