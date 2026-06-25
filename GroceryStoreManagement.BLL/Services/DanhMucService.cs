using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class DanhMucService
{
    private readonly DanhMucRepository _repository;

    public DanhMucService(DanhMucRepository repository)
    {
        _repository = repository;
    }

    public Task<List<DanhMuc>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<int> AddAsync(DanhMuc danhMuc)
    {
        await ValidateAsync(danhMuc);
        return await _repository.AddAsync(danhMuc);
    }

    public async Task UpdateAsync(DanhMuc danhMuc)
    {
        if (danhMuc.Id <= 0)
        {
            throw new InvalidOperationException("Danh muc khong hop le.");
        }

        await ValidateAsync(danhMuc, danhMuc.Id);
        await _repository.UpdateAsync(danhMuc);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("Danh muc khong hop le.");
        }

        return _repository.DeleteAsync(id);
    }

    private async Task ValidateAsync(DanhMuc danhMuc, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(danhMuc.MaDanhMuc))
        {
            throw new InvalidOperationException("Ma danh muc la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(danhMuc.TenDanhMuc))
        {
            throw new InvalidOperationException("Ten danh muc la bat buoc.");
        }

        if (await _repository.IsDuplicateAsync(danhMuc.MaDanhMuc, excludeId))
        {
            throw new InvalidOperationException("Ma danh muc da ton tai.");
        }
    }
}
