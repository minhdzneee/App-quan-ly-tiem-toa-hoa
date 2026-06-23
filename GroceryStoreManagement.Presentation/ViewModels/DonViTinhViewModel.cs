using System.Collections.ObjectModel;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class DonViTinhViewModel : BaseViewModel
{
    private readonly DonViTinhService _service;

    public DonViTinhViewModel(DonViTinhService service)
    {
        _service = service;
    }

    public ObservableCollection<DonViTinh> DonViTinhs { get; } = new();

    public async Task LoadAsync()
    {
        DonViTinhs.Clear();
        foreach (var item in await _service.GetAllAsync())
        {
            DonViTinhs.Add(item);
        }
    }
}
