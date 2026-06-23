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
}
