using System.Collections.ObjectModel;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class NhaCungCapViewModel : BaseViewModel
{
    private readonly NhaCungCapService _service;

    public NhaCungCapViewModel(NhaCungCapService service)
    {
        _service = service;
    }

    public ObservableCollection<NhaCungCap> NhaCungCaps { get; } = new();

    public async Task LoadAsync()
    {
        NhaCungCaps.Clear();
        foreach (var item in await _service.GetAllAsync())
        {
            NhaCungCaps.Add(item);
        }
    }
}
