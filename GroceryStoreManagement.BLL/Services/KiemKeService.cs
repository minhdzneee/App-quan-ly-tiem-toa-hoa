using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class KiemKeService
{
    private readonly KiemKeRepository _repository;

    public KiemKeService(KiemKeRepository repository)
    {
        _repository = repository;
    }

    public Task<int> LapPhieuKiemKeAsync(PhieuKiemKe phieuKiemKe)
    {
        if (phieuKiemKe.NhanVienId <= 0)
        {
            throw new InvalidOperationException("Phieu kiem ke can co nhan vien.");
        }

        if (phieuKiemKe.ChiTietKiemKes.Count == 0)
        {
            throw new InvalidOperationException("Phieu kiem ke phai co chi tiet.");
        }

        foreach (var item in phieuKiemKe.ChiTietKiemKes)
        {
            if (item.SanPhamId <= 0 || item.SoLuongHeThong < 0 || item.SoLuongThucTe < 0)
            {
                throw new InvalidOperationException("Chi tiet kiem ke khong hop le.");
            }
        }

        phieuKiemKe.MaPhieuKiemKe = string.IsNullOrWhiteSpace(phieuKiemKe.MaPhieuKiemKe)
            ? $"KK{DateTime.Now:yyyyMMddHHmmss}"
            : phieuKiemKe.MaPhieuKiemKe;

        return _repository.CreateAsync(phieuKiemKe);
    }
}
