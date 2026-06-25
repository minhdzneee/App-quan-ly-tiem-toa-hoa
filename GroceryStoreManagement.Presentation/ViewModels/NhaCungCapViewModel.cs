using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class NhaCungCapViewModel : BaseViewModel
{
    private readonly NhaCungCapService _service;
    private NhaCungCap? _selectedNhaCungCap;
    private NhaCungCap _editingNhaCungCap = new();
    private string _statusMessage = string.Empty;

    public NhaCungCapViewModel(NhaCungCapService service)
    {
        _service = service;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        NewCommand = new RelayCommand(_ => NewNhaCungCap());
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => SelectedNhaCungCap is not null);
        LoadCommand.Execute(null);
    }

    public ObservableCollection<NhaCungCap> NhaCungCaps { get; } = new();

    public NhaCungCap? SelectedNhaCungCap
    {
        get => _selectedNhaCungCap;
        set
        {
            if (SetProperty(ref _selectedNhaCungCap, value) && value is not null)
            {
                EditingNhaCungCap = Clone(value);
            }

            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public NhaCungCap EditingNhaCungCap
    {
        get => _editingNhaCungCap;
        set => SetProperty(ref _editingNhaCungCap, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            NhaCungCaps.Clear();
            foreach (var item in await _service.GetAllAsync())
            {
                NhaCungCaps.Add(item);
            }

            NewNhaCungCap(updateStatus: false);
            StatusMessage = $"Tải {NhaCungCaps.Count} nhà cung cấp.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void NewNhaCungCap(bool updateStatus = true)
    {
        EditingNhaCungCap = new NhaCungCap();
        SelectedNhaCungCap = null;
        if (updateStatus)
        {
            StatusMessage = "Đang thêm nhà cung cấp mới.";
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (EditingNhaCungCap.Id == 0)
            {
                await _service.AddAsync(EditingNhaCungCap);
                StatusMessage = "Đã thêm nhà cung cấp.";
            }
            else
            {
                await _service.UpdateAsync(EditingNhaCungCap);
                StatusMessage = "Đã cập nhật nhà cung cấp.";
            }

            var successMessage = StatusMessage;
            await LoadAsync();
            StatusMessage = successMessage;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedNhaCungCap is null)
        {
            return;
        }

        try
        {
            await _service.DeleteAsync(SelectedNhaCungCap.Id);
            await LoadAsync();
            StatusMessage = "Đã xóa nhà cung cấp.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private static NhaCungCap Clone(NhaCungCap item)
    {
        return new NhaCungCap
        {
            Id = item.Id,
            MaNhaCungCap = item.MaNhaCungCap,
            TenNhaCungCap = item.TenNhaCungCap,
            SoDienThoai = item.SoDienThoai,
            DiaChi = item.DiaChi,
            Email = item.Email
        };
    }
}
