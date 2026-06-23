using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class BaoCaoViewModel : BaseViewModel
{
    private readonly BaoCaoService _baoCaoService;
    private DateTime _fromDate = DateTime.Today.AddDays(-30);
    private DateTime _toDate = DateTime.Today;
    private DashboardStats _stats = new();
    private string _statusMessage = string.Empty;

    public BaoCaoViewModel(BaoCaoService baoCaoService)
    {
        _baoCaoService = baoCaoService;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        LoadCommand.Execute(null);
    }

    public ObservableCollection<SanPhamBanChay> TopSanPhamBanChay { get; } = new();
    public ObservableCollection<SanPham> SanPhamTonKhoThap { get; } = new();
    public ObservableCollection<SanPham> SanPhamSapHetHan { get; } = new();

    public DateTime FromDate
    {
        get => _fromDate;
        set => SetProperty(ref _fromDate, value);
    }

    public DateTime ToDate
    {
        get => _toDate;
        set => SetProperty(ref _toDate, value);
    }

    public DashboardStats Stats
    {
        get => _stats;
        set => SetProperty(ref _stats, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            Stats = await _baoCaoService.GetDashboardStatsAsync();

            TopSanPhamBanChay.Clear();
            foreach (var item in await _baoCaoService.GetTopSanPhamBanChayAsync(FromDate, ToDate))
            {
                TopSanPhamBanChay.Add(item);
            }

            SanPhamTonKhoThap.Clear();
            foreach (var item in await _baoCaoService.GetSanPhamTonKhoThapAsync())
            {
                SanPhamTonKhoThap.Add(item);
            }

            SanPhamSapHetHan.Clear();
            foreach (var item in await _baoCaoService.GetSanPhamSapHetHanAsync())
            {
                SanPhamSapHetHan.Add(item);
            }

            StatusMessage = "Đã tải báo cáo.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
