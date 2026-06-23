using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class KiemKeViewModel : BaseViewModel
{
    private readonly KiemKeService _kiemKeService;
    private readonly SanPhamService _sanPhamService;
    private readonly int _nhanVienId;
    private string _searchKeyword = string.Empty;
    private SanPham? _selectedSanPham;
    private int _soLuongThucTe;
    private string _statusMessage = string.Empty;

    public KiemKeViewModel(KiemKeService kiemKeService, SanPhamService sanPhamService, int nhanVienId)
    {
        _kiemKeService = kiemKeService;
        _sanPhamService = sanPhamService;
        _nhanVienId = nhanVienId;

        SearchProductCommand = new AsyncRelayCommand(_ => SearchProductAsync());
        AddItemCommand = new RelayCommand(_ => AddItem(), _ => SelectedSanPham is not null);
        RemoveItemCommand = new RelayCommand(item => RemoveItem(item as ChiTietKiemKe), item => item is ChiTietKiemKe);
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => DetailItems.Count > 0);
        ClearCommand = new RelayCommand(_ => Clear());
    }

    public ObservableCollection<ChiTietKiemKe> DetailItems { get; } = new();

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public SanPham? SelectedSanPham
    {
        get => _selectedSanPham;
        set
        {
            if (SetProperty(ref _selectedSanPham, value) && value is not null)
            {
                SoLuongThucTe = value.SoLuongTon;
            }
        }
    }

    public int SoLuongThucTe
    {
        get => _soLuongThucTe;
        set => SetProperty(ref _soLuongThucTe, value < 0 ? 0 : value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SearchProductCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    private async Task SearchProductAsync()
    {
        try
        {
            SelectedSanPham = await _sanPhamService.GetByCodeOrBarcodeAsync(SearchKeyword);
            StatusMessage = SelectedSanPham is null
                ? "Không tìm thấy sản phẩm."
                : $"Đã chọn {SelectedSanPham.TenSanPham}.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void AddItem()
    {
        if (SelectedSanPham is null)
        {
            return;
        }

        var detail = new ChiTietKiemKe
        {
            SanPhamId = SelectedSanPham.Id,
            MaSanPham = SelectedSanPham.MaSanPham,
            TenSanPham = SelectedSanPham.TenSanPham,
            SoLuongHeThong = SelectedSanPham.SoLuongTon,
            SoLuongThucTe = SoLuongThucTe
        };
        detail.TinhChenhLech();
        DetailItems.Add(detail);
        StatusMessage = "Đã thêm dòng kiểm kê.";
    }

    private void RemoveItem(ChiTietKiemKe? item)
    {
        if (item is null)
        {
            return;
        }

        DetailItems.Remove(item);
    }

    private async Task SaveAsync()
    {
        try
        {
            var phieuKiemKe = new PhieuKiemKe
            {
                NhanVienId = _nhanVienId,
                ChiTietKiemKes = DetailItems.Select(item => new ChiTietKiemKe
                {
                    SanPhamId = item.SanPhamId,
                    SoLuongHeThong = item.SoLuongHeThong,
                    SoLuongThucTe = item.SoLuongThucTe
                }).ToList()
            };

            await _kiemKeService.LapPhieuKiemKeAsync(phieuKiemKe);
            StatusMessage = $"Đã lưu phiếu kiểm kê {phieuKiemKe.MaPhieuKiemKe}.";
            Clear();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void Clear()
    {
        DetailItems.Clear();
        SearchKeyword = string.Empty;
        SelectedSanPham = null;
        SoLuongThucTe = 0;
    }
}
