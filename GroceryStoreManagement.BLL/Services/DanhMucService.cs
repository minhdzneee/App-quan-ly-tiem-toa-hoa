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
}
