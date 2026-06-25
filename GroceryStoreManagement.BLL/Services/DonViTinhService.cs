using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class DonViTinhService
{
    private readonly DonViTinhRepository _repository;

    public DonViTinhService(DonViTinhRepository repository)
    {
        _repository = repository;
    }

    public Task<List<DonViTinh>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<int> AddAsync(DonViTinh donViTinh)
    {
        await ValidateAsync(donViTinh);
        return await _repository.AddAsync(donViTinh);
    }

    public async Task UpdateAsync(DonViTinh donViTinh)
    {
        if (donViTinh.Id <= 0)
        {
            throw new InvalidOperationException("Don vi tinh khong hop le.");
        }

        await ValidateAsync(donViTinh, donViTinh.Id);
        await _repository.UpdateAsync(donViTinh);
    }

    public Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException("Don vi tinh khong hop le.");
        }

        return _repository.DeleteAsync(id);
    }

    private async Task ValidateAsync(DonViTinh donViTinh, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(donViTinh.TenDonVi))
        {
            throw new InvalidOperationException("Ten don vi tinh la bat buoc.");
        }

        if (await _repository.IsDuplicateAsync(donViTinh.TenDonVi, excludeId))
        {
            throw new InvalidOperationException("Ten don vi tinh da ton tai.");
        }
    }
}
