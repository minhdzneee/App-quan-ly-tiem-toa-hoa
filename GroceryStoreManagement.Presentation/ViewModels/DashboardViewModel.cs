using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly BaoCaoService _baoCaoService;
    private DashboardStats _stats = new();
    private string _statusMessage = string.Empty;

    public DashboardViewModel(BaoCaoService baoCaoService)
    {
        _baoCaoService = baoCaoService;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        LoadCommand.Execute(null);
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
            StatusMessage = "Dữ liệu dashboard đã được cập nhật.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
