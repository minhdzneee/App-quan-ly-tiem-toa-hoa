using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class NhapHangViewModel : BaseViewModel
{
    private readonly NhapHangService _nhapHangService;
    private readonly NhaCungCapService _nhaCungCapService;
    private readonly SanPhamService _sanPhamService;
    private readonly int _nhanVienId;
    private NhaCungCap? _selectedNhaCungCap;
    private SanPham? _selectedSanPham;
    private int _soLuong = 1;
    private decimal _donGiaNhap;
    private string _productKeyword = string.Empty;
    private string _statusMessage = string.Empty;

    public NhapHangViewModel(
        NhapHangService nhapHangService,
        NhaCungCapService nhaCungCapService,
        SanPhamService sanPhamService,
        int nhanVienId)
    {
        _nhapHangService = nhapHangService;
        _nhaCungCapService = nhaCungCapService;
        _sanPhamService = sanPhamService;
        _nhanVienId = nhanVienId;

        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        SearchProductCommand = new AsyncRelayCommand(_ => SearchProductAsync());
        AddItemCommand = new RelayCommand(_ => AddItem(), _ => SelectedSanPham is not null);
        RemoveItemCommand = new RelayCommand(item => RemoveItem(item as ChiTietPhieuNhap), item => item is ChiTietPhieuNhap);
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => DetailItems.Count > 0);
        ClearCommand = new RelayCommand(_ => Clear());

        LoadCommand.Execute(null);
    }

    public ObservableCollection<NhaCungCap> NhaCungCaps { get; } = new();
    public ObservableCollection<ChiTietPhieuNhap> DetailItems { get; } = new();

    public NhaCungCap? SelectedNhaCungCap
    {
        get => _selectedNhaCungCap;
        set => SetProperty(ref _selectedNhaCungCap, value);
    }

    public SanPham? SelectedSanPham
    {
        get => _selectedSanPham;
        set
        {
            if (SetProperty(ref _selectedSanPham, value) && value is not null)
            {
                DonGiaNhap = value.GiaNhap;
            }
        }
    }

    public int SoLuong
    {
        get => _soLuong;
        set => SetProperty(ref _soLuong, value <= 0 ? 1 : value);
    }

    public decimal DonGiaNhap
    {
        get => _donGiaNhap;
        set => SetProperty(ref _donGiaNhap, value < 0 ? 0 : value);
    }

    public string ProductKeyword
    {
        get => _productKeyword;
        set => SetProperty(ref _productKeyword, value);
    }

    public decimal TongTien => DetailItems.Sum(item => item.ThanhTien);

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SearchProductCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            NhaCungCaps.Clear();
            foreach (var supplier in await _nhaCungCapService.GetAllAsync())
            {
                NhaCungCaps.Add(supplier);
            }

            SelectedNhaCungCap = NhaCungCaps.FirstOrDefault();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task SearchProductAsync()
    {
        try
        {
            SelectedSanPham = await _sanPhamService.GetByCodeOrBarcodeAsync(ProductKeyword);
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

        var detail = new ChiTietPhieuNhap
        {
            SanPhamId = SelectedSanPham.Id,
            MaSanPham = SelectedSanPham.MaSanPham,
            TenSanPham = SelectedSanPham.TenSanPham,
            SoLuong = SoLuong,
            DonGiaNhap = DonGiaNhap
        };
        detail.TinhThanhTien();
        DetailItems.Add(detail);
        OnPropertyChanged(nameof(TongTien));
        StatusMessage = "Đã thêm sản phẩm vào phiếu nhập.";
    }

    private void RemoveItem(ChiTietPhieuNhap? item)
    {
        if (item is null)
        {
            return;
        }

        DetailItems.Remove(item);
        OnPropertyChanged(nameof(TongTien));
    }

    private async Task SaveAsync()
    {
        try
        {
            var phieuNhap = new PhieuNhap
            {
                NhaCungCapId = SelectedNhaCungCap?.Id ?? 0,
                NhanVienId = _nhanVienId,
                ChiTietPhieuNhaps = DetailItems.Select(item => new ChiTietPhieuNhap
                {
                    SanPhamId = item.SanPhamId,
                    SoLuong = item.SoLuong,
                    DonGiaNhap = item.DonGiaNhap
                }).ToList()
            };

            await _nhapHangService.LapPhieuNhapAsync(phieuNhap);
            StatusMessage = $"Đã lưu phiếu nhập {phieuNhap.MaPhieuNhap}.";
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
        SelectedSanPham = null;
        ProductKeyword = string.Empty;
        SoLuong = 1;
        DonGiaNhap = 0;
        OnPropertyChanged(nameof(TongTien));
    }
}
