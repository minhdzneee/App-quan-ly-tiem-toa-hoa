using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class SanPhamService
{
    private readonly SanPhamRepository _repository;

    public SanPhamService(SanPhamRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SanPham>> SearchAsync(string? keyword = null, int? danhMucId = null)
    {
        return _repository.SearchAsync(keyword, danhMucId);
    }

    public Task<SanPham?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<SanPham?> GetByCodeOrBarcodeAsync(string keyword)
    {
        return _repository.GetByCodeOrBarcodeAsync(keyword);
    }

    public async Task<int> AddAsync(SanPham sanPham)
    {
        await ValidateAsync(sanPham);
        return await _repository.AddAsync(sanPham);
    }

    public async Task UpdateAsync(SanPham sanPham)
    {
        if (sanPham.Id <= 0)
        {
            throw new InvalidOperationException("San pham can co Id khi cap nhat.");
        }

        await ValidateAsync(sanPham, sanPham.Id);
        await _repository.UpdateAsync(sanPham);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("San pham khong hop le.");
        }

        return _repository.DeleteAsync(id);
    }

    private async Task ValidateAsync(SanPham sanPham, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(sanPham.MaSanPham))
        {
            throw new InvalidOperationException("Ma san pham la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(sanPham.TenSanPham))
        {
            throw new InvalidOperationException("Ten san pham la bat buoc.");
        }

        if (sanPham.GiaNhap < 0 || sanPham.GiaBan < 0)
        {
            throw new InvalidOperationException("Gia tien khong duoc am.");
        }

        if (sanPham.SoLuongTon < 0 || sanPham.TonToiThieu < 0)
        {
            throw new InvalidOperationException("So luong khong duoc am.");
        }

        if (await _repository.IsDuplicateAsync(sanPham.MaSanPham, sanPham.MaVach, excludeId))
        {
            throw new InvalidOperationException("Ma san pham hoac ma vach da ton tai.");
        }
    }
}
