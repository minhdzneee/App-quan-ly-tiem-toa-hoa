using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class KhachHangService
{
    private readonly KhachHangRepository _repository;

    public KhachHangService(KhachHangRepository repository)
    {
        _repository = repository;
    }

    public Task<List<KhachHang>> SearchAsync(string? keyword)
    {
        return _repository.SearchAsync(keyword);
    }

    public async Task<int> AddAsync(KhachHang khachHang)
    {
        await ValidateAsync(khachHang);
        return await _repository.AddAsync(khachHang);
    }

    public async Task UpdateAsync(KhachHang khachHang)
    {
        if (khachHang.Id <= 0)
        {
            throw new InvalidOperationException("Khach hang khong hop le.");
        }

        await ValidateAsync(khachHang, khachHang.Id);
        await _repository.UpdateAsync(khachHang);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("Khach hang khong hop le.");
        }

        return _repository.DeleteAsync(id);
    }

    private async Task ValidateAsync(KhachHang khachHang, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(khachHang.MaKhachHang))
        {
            throw new InvalidOperationException("Ma khach hang la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(khachHang.HoTen))
        {
            throw new InvalidOperationException("Ho ten khach hang la bat buoc.");
        }

        if (khachHang.DiemTichLuy < 0)
        {
            throw new InvalidOperationException("Diem tich luy khong duoc am.");
        }

        if (await _repository.IsDuplicateAsync(khachHang.MaKhachHang, excludeId))
        {
            throw new InvalidOperationException("Ma khach hang da ton tai.");
        }
    }
}
