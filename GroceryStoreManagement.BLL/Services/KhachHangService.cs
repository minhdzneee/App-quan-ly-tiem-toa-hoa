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
}
