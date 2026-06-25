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
    private KhachHang? _selectedKhachHang;
    private KhachHang _editingKhachHang = new();
    private string _statusMessage = string.Empty;

    public KhachHangViewModel(KhachHangService khachHangService)
    {
        _khachHangService = khachHangService;
        SearchCommand = new AsyncRelayCommand(_ => SearchAsync());
        NewCommand = new RelayCommand(_ => NewKhachHang());
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => SelectedKhachHang is not null);
        SearchCommand.Execute(null);
    }

    public ObservableCollection<KhachHang> KhachHangs { get; } = new();

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public KhachHang? SelectedKhachHang
    {
        get => _selectedKhachHang;
        set
        {
            if (SetProperty(ref _selectedKhachHang, value) && value is not null)
            {
                EditingKhachHang = Clone(value);
            }

            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public KhachHang EditingKhachHang
    {
        get => _editingKhachHang;
        set => SetProperty(ref _editingKhachHang, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    private async Task SearchAsync(bool updateStatus = true)
    {
        try
        {
            KhachHangs.Clear();
            foreach (var customer in await _khachHangService.SearchAsync(SearchKeyword))
            {
                KhachHangs.Add(customer);
            }

            if (updateStatus)
            {
                StatusMessage = $"Tìm thấy {KhachHangs.Count} khách hàng.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void NewKhachHang()
    {
        EditingKhachHang = new KhachHang
        {
            DiemTichLuy = 0
        };
        SelectedKhachHang = null;
        StatusMessage = "Đang thêm khách hàng mới.";
    }

    private async Task SaveAsync()
    {
        try
        {
            if (EditingKhachHang.Id == 0)
            {
                await _khachHangService.AddAsync(EditingKhachHang);
                StatusMessage = "Đã thêm khách hàng.";
            }
            else
            {
                await _khachHangService.UpdateAsync(EditingKhachHang);
                StatusMessage = "Đã cập nhật khách hàng.";
            }

            var successMessage = StatusMessage;
            await SearchAsync(updateStatus: false);
            NewKhachHang();
            StatusMessage = successMessage;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedKhachHang is null)
        {
            return;
        }

        try
        {
            await _khachHangService.DeleteAsync(SelectedKhachHang.Id);
            await SearchAsync(updateStatus: false);
            NewKhachHang();
            StatusMessage = "Đã xóa khách hàng.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private static KhachHang Clone(KhachHang khachHang)
    {
        return new KhachHang
        {
            Id = khachHang.Id,
            MaKhachHang = khachHang.MaKhachHang,
            HoTen = khachHang.HoTen,
            SoDienThoai = khachHang.SoDienThoai,
            DiaChi = khachHang.DiaChi,
            DiemTichLuy = khachHang.DiemTichLuy
        };
    }
}
