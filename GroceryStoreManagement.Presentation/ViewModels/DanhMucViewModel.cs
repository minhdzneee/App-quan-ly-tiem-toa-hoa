using System.Collections.ObjectModel;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class DanhMucViewModel : BaseViewModel
{
    private readonly DanhMucService _service;

    public DanhMucViewModel(DanhMucService service)
    {
        _service = service;
    }

    public ObservableCollection<DanhMuc> DanhMucs { get; } = new();

    public async Task LoadAsync()
    {
        DanhMucs.Clear();
        foreach (var item in await _service.GetAllAsync())
        {
            DanhMucs.Add(item);
        }
    }
}
