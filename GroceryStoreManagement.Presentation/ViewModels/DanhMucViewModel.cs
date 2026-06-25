using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class DanhMucViewModel : BaseViewModel
{
    private readonly DanhMucService _service;
    private DanhMuc? _selectedDanhMuc;
    private DanhMuc _editingDanhMuc = new();
    private string _statusMessage = string.Empty;

    public DanhMucViewModel(DanhMucService service)
    {
        _service = service;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        NewCommand = new RelayCommand(_ => NewDanhMuc());
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => SelectedDanhMuc is not null);
        LoadCommand.Execute(null);
    }

    public ObservableCollection<DanhMuc> DanhMucs { get; } = new();

    public DanhMuc? SelectedDanhMuc
    {
        get => _selectedDanhMuc;
        set
        {
            if (SetProperty(ref _selectedDanhMuc, value) && value is not null)
            {
                EditingDanhMuc = Clone(value);
            }

            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public DanhMuc EditingDanhMuc
    {
        get => _editingDanhMuc;
        set => SetProperty(ref _editingDanhMuc, value);
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
            DanhMucs.Clear();
            foreach (var item in await _service.GetAllAsync())
            {
                DanhMucs.Add(item);
            }

            NewDanhMuc(updateStatus: false);
            StatusMessage = $"Tải {DanhMucs.Count} danh mục.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void NewDanhMuc(bool updateStatus = true)
    {
        EditingDanhMuc = new DanhMuc();
        SelectedDanhMuc = null;
        if (updateStatus)
        {
            StatusMessage = "Đang thêm danh mục mới.";
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (EditingDanhMuc.Id == 0)
            {
                await _service.AddAsync(EditingDanhMuc);
                StatusMessage = "Đã thêm danh mục.";
            }
            else
            {
                await _service.UpdateAsync(EditingDanhMuc);
                StatusMessage = "Đã cập nhật danh mục.";
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
        if (SelectedDanhMuc is null)
        {
            return;
        }

        try
        {
            await _service.DeleteAsync(SelectedDanhMuc.Id);
            await LoadAsync();
            StatusMessage = "Đã xóa danh mục.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private static DanhMuc Clone(DanhMuc item)
    {
        return new DanhMuc
        {
            Id = item.Id,
            MaDanhMuc = item.MaDanhMuc,
            TenDanhMuc = item.TenDanhMuc,
            MoTa = item.MoTa
        };
    }
}
