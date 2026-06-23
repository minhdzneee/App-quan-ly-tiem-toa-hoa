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
}
