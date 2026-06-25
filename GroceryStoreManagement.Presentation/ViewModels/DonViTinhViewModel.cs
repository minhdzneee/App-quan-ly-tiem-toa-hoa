using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class DonViTinhViewModel : BaseViewModel
{
    private readonly DonViTinhService _service;
    private DonViTinh? _selectedDonViTinh;
    private DonViTinh _editingDonViTinh = new();
    private string _statusMessage = string.Empty;

    public DonViTinhViewModel(DonViTinhService service)
    {
        _service = service;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        NewCommand = new RelayCommand(_ => NewDonViTinh());
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => SelectedDonViTinh is not null);
        LoadCommand.Execute(null);
    }

    public ObservableCollection<DonViTinh> DonViTinhs { get; } = new();

    public DonViTinh? SelectedDonViTinh
    {
        get => _selectedDonViTinh;
        set
        {
            if (SetProperty(ref _selectedDonViTinh, value) && value is not null)
            {
                EditingDonViTinh = new DonViTinh
                {
                    Id = value.Id,
                    TenDonVi = value.TenDonVi
                };
            }

            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public DonViTinh EditingDonViTinh
    {
        get => _editingDonViTinh;
        set => SetProperty(ref _editingDonViTinh, value);
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
            DonViTinhs.Clear();
            foreach (var item in await _service.GetAllAsync())
            {
                DonViTinhs.Add(item);
            }

            NewDonViTinh(updateStatus: false);
            StatusMessage = $"Tải {DonViTinhs.Count} đơn vị tính.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void NewDonViTinh(bool updateStatus = true)
    {
        EditingDonViTinh = new DonViTinh();
        SelectedDonViTinh = null;
        if (updateStatus)
        {
            StatusMessage = "Đang thêm đơn vị tính mới.";
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (EditingDonViTinh.Id == 0)
            {
                await _service.AddAsync(EditingDonViTinh);
                StatusMessage = "Đã thêm đơn vị tính.";
            }
            else
            {
                await _service.UpdateAsync(EditingDonViTinh);
                StatusMessage = "Đã cập nhật đơn vị tính.";
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
        if (SelectedDonViTinh is null)
        {
            return;
        }

        try
        {
            await _service.DeleteAsync(SelectedDonViTinh.Id);
            await LoadAsync();
            StatusMessage = "Đã xóa đơn vị tính.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
