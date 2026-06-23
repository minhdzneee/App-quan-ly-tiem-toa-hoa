using GroceryStoreManagement.DAL.Repositories;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.BLL.Services;

public class BaoCaoService
{
    private readonly BaoCaoRepository _repository;

    public BaoCaoService(BaoCaoRepository repository)
    {
        _repository = repository;
    }

    public Task<DashboardStats> GetDashboardStatsAsync()
    {
        return _repository.GetDashboardStatsAsync();
    }

    public Task<List<SanPhamBanChay>> GetTopSanPhamBanChayAsync(DateTime fromDate, DateTime toDate, int top = 10)
    {
        return _repository.GetTopSanPhamBanChayAsync(fromDate, toDate, top);
    }

    public Task<List<SanPham>> GetSanPhamTonKhoThapAsync()
    {
        return _repository.GetSanPhamTonKhoThapAsync();
    }

    public Task<List<SanPham>> GetSanPhamSapHetHanAsync(int soNgay = 30)
    {
        return _repository.GetSanPhamSapHetHanAsync(soNgay);
    }
}
