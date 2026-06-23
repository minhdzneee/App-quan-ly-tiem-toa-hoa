using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class NhapHangService
{
    private readonly PhieuNhapRepository _phieuNhapRepository;

    public NhapHangService(PhieuNhapRepository phieuNhapRepository)
    {
        _phieuNhapRepository = phieuNhapRepository;
    }

    public async Task<int> LapPhieuNhapAsync(PhieuNhap phieuNhap)
    {
        if (phieuNhap.NhaCungCapId <= 0)
        {
            throw new InvalidOperationException("Vui long chon nha cung cap.");
        }

        if (phieuNhap.NhanVienId <= 0)
        {
            throw new InvalidOperationException("Vui long chon nhan vien nhap hang.");
        }

        if (phieuNhap.ChiTietPhieuNhaps.Count == 0)
        {
            throw new InvalidOperationException("Phieu nhap phai co it nhat mot san pham.");
        }

        foreach (var item in phieuNhap.ChiTietPhieuNhaps)
        {
            if (item.SanPhamId <= 0 || item.SoLuong <= 0 || item.DonGiaNhap < 0)
            {
                throw new InvalidOperationException("Chi tiet phieu nhap khong hop le.");
            }
        }

        phieuNhap.MaPhieuNhap = string.IsNullOrWhiteSpace(phieuNhap.MaPhieuNhap)
            ? GenerateCode("PN")
            : phieuNhap.MaPhieuNhap;
        phieuNhap.NgayNhap = phieuNhap.NgayNhap == default ? DateTime.Now : phieuNhap.NgayNhap;

        return await _phieuNhapRepository.CreateAsync(phieuNhap);
    }

    public Task<List<PhieuNhap>> GetRecentAsync(int top = 50)
    {
        return _phieuNhapRepository.GetRecentAsync(top);
    }

    private static string GenerateCode(string prefix)
    {
        return $"{prefix}{DateTime.Now:yyyyMMddHHmmss}";
    }
}
