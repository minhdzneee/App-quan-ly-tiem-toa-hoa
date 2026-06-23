using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class KhachHangViewModel : BaseViewModel
{
    private readonly KhachHangService _khachHangService;
    private string _searchKeyword = string.Empty;
    private string _statusMessage = string.Empty;

    public KhachHangViewModel(KhachHangService khachHangService)
    {
        _khachHangService = khachHangService;
        SearchCommand = new AsyncRelayCommand(_ => SearchAsync());
        SearchCommand.Execute(null);
    }

    public ObservableCollection<KhachHang> KhachHangs { get; } = new();

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SearchCommand { get; }

    private async Task SearchAsync()
    {
        try
        {
            KhachHangs.Clear();
            foreach (var customer in await _khachHangService.SearchAsync(SearchKeyword))
            {
                KhachHangs.Add(customer);
            }

            StatusMessage = $"Tìm thấy {KhachHangs.Count} khách hàng.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
