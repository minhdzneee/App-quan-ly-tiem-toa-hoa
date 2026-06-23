using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class NhanVienService
{
    private readonly NhanVienRepository _repository;

    public NhanVienService(NhanVienRepository repository)
    {
        _repository = repository;
    }

    public Task<List<NhanVien>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }
}
