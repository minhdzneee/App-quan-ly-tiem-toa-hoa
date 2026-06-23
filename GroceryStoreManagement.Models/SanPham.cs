namespace GroceryStoreManagement.Models;

public class SanPham
{
    public int Id { get; set; }
    public string MaSanPham { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public int DanhMucId { get; set; }
    public string? TenDanhMuc { get; set; }
    public int DonViTinhId { get; set; }
    public string? TenDonVi { get; set; }
    public decimal GiaNhap { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuongTon { get; set; }
    public int TonToiThieu { get; set; }
    public DateTime? HanSuDung { get; set; }
    public string? MaVach { get; set; }
    public string TrangThai { get; set; } = "DangBan";

    public bool KiemTraTonKho(int soLuongCanBan)
    {
        return soLuongCanBan > 0 && SoLuongTon >= soLuongCanBan;
    }

    public void CapNhatSoLuong(int soLuongThayDoi)
    {
        var soLuongMoi = SoLuongTon + soLuongThayDoi;
        if (soLuongMoi < 0)
        {
            throw new InvalidOperationException("Stock quantity cannot be negative.");
        }

        SoLuongTon = soLuongMoi;
    }

    public bool KiemTraHetHan(DateTime? ngayKiemTra = null)
    {
        var today = (ngayKiemTra ?? DateTime.Today).Date;
        return HanSuDung.HasValue && HanSuDung.Value.Date < today;
    }

    public bool SapHetHan(int soNgayCanhBao = 30, DateTime? ngayKiemTra = null)
    {
        if (!HanSuDung.HasValue)
        {
            return false;
        }

        var today = (ngayKiemTra ?? DateTime.Today).Date;
        return HanSuDung.Value.Date >= today && HanSuDung.Value.Date <= today.AddDays(soNgayCanhBao);
    }

    public bool TonKhoThap => SoLuongTon <= TonToiThieu;
}
