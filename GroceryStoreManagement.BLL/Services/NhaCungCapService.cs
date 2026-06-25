using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class NhaCungCapService
{
    private readonly NhaCungCapRepository _repository;

    public NhaCungCapService(NhaCungCapRepository repository)
    {
        _repository = repository;
    }

    public Task<List<NhaCungCap>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<int> AddAsync(NhaCungCap nhaCungCap)
    {
        await ValidateAsync(nhaCungCap);
        return await _repository.AddAsync(nhaCungCap);
    }

    public async Task UpdateAsync(NhaCungCap nhaCungCap)
    {
        if (nhaCungCap.Id <= 0)
        {
            throw new InvalidOperationException("Nha cung cap khong hop le.");
        }

        await ValidateAsync(nhaCungCap, nhaCungCap.Id);
        await _repository.UpdateAsync(nhaCungCap);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("Nha cung cap khong hop le.");
        }

        return _repository.DeleteAsync(id);
    }

    private async Task ValidateAsync(NhaCungCap nhaCungCap, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(nhaCungCap.MaNhaCungCap))
        {
            throw new InvalidOperationException("Ma nha cung cap la bat buoc.");
        }

        if (string.IsNullOrWhiteSpace(nhaCungCap.TenNhaCungCap))
        {
            throw new InvalidOperationException("Ten nha cung cap la bat buoc.");
        }

        if (await _repository.IsDuplicateAsync(nhaCungCap.MaNhaCungCap, excludeId))
        {
            throw new InvalidOperationException("Ma nha cung cap da ton tai.");
        }
    }
}
