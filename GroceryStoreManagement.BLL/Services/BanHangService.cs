using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class BanHangService
{
    private readonly HoaDonRepository _hoaDonRepository;
    private readonly SanPhamRepository _sanPhamRepository;

    public BanHangService(HoaDonRepository hoaDonRepository, SanPhamRepository sanPhamRepository)
    {
        _hoaDonRepository = hoaDonRepository;
        _sanPhamRepository = sanPhamRepository;
    }

    public Task<SanPham?> TimSanPhamAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new InvalidOperationException("Vui long nhap ma san pham, ten san pham hoac ma vach.");
        }

        return _sanPhamRepository.GetByCodeOrBarcodeAsync(keyword.Trim());
    }

    public async Task<int> ThanhToanAsync(HoaDon hoaDon)
    {
        if (hoaDon.NhanVienId <= 0)
        {
            throw new InvalidOperationException("Hoa don can co nhan vien lap.");
        }

        if (hoaDon.ChiTietHoaDons.Count == 0)
        {
            throw new InvalidOperationException("Hoa don phai co it nhat mot san pham.");
        }

        foreach (var item in hoaDon.ChiTietHoaDons)
        {
            if (item.SanPhamId <= 0 || item.SoLuong <= 0 || item.DonGia < 0)
            {
                throw new InvalidOperationException("Chi tiet hoa don khong hop le.");
            }

            var sanPham = await _sanPhamRepository.GetByIdAsync(item.SanPhamId);
            if (sanPham is null)
            {
                throw new InvalidOperationException("San pham khong ton tai.");
            }

            if (!sanPham.KiemTraTonKho(item.SoLuong))
            {
                throw new InvalidOperationException($"San pham {sanPham.TenSanPham} khong du ton kho.");
            }
        }

        hoaDon.MaHoaDon = string.IsNullOrWhiteSpace(hoaDon.MaHoaDon)
            ? GenerateCode("HD")
            : hoaDon.MaHoaDon;
        hoaDon.NgayLap = hoaDon.NgayLap == default ? DateTime.Now : hoaDon.NgayLap;
        hoaDon.TinhTongTien();

        return await _hoaDonRepository.CreateAsync(hoaDon);
    }

    public Task<List<HoaDon>> GetRecentAsync(int top = 50)
    {
        return _hoaDonRepository.GetRecentAsync(top);
    }

    private static string GenerateCode(string prefix)
    {
        return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}";
    }
}
