using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class BanHangViewModel : BaseViewModel
{
    private readonly BanHangService _banHangService;
    private readonly KhachHangService _khachHangService;
    private readonly int _nhanVienId;
    private string _searchKeyword = string.Empty;
    private int _soLuong = 1;
    private decimal _giamGia;
    private SanPham? _selectedSanPham;
    private KhachHang? _selectedKhachHang;
    private string _statusMessage = string.Empty;

    public BanHangViewModel(BanHangService banHangService, KhachHangService khachHangService, int nhanVienId)
    {
        _banHangService = banHangService;
        _khachHangService = khachHangService;
        _nhanVienId = nhanVienId;

        SearchProductCommand = new AsyncRelayCommand(_ => SearchProductAsync());
        LoadCustomersCommand = new AsyncRelayCommand(_ => LoadCustomersAsync());
        AddToInvoiceCommand = new RelayCommand(_ => AddToInvoice(), _ => SelectedSanPham is not null);
        RemoveItemCommand = new RelayCommand(item => RemoveItem(item as ChiTietHoaDon), item => item is ChiTietHoaDon);
        CheckoutCommand = new AsyncRelayCommand(_ => CheckoutAsync(), _ => InvoiceItems.Count > 0);
        ClearCommand = new RelayCommand(_ => ClearInvoice());

        LoadCustomersCommand.Execute(null);
    }

    public ObservableCollection<ChiTietHoaDon> InvoiceItems { get; } = new();
    public ObservableCollection<KhachHang> KhachHangs { get; } = new();

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public int SoLuong
    {
        get => _soLuong;
        set => SetProperty(ref _soLuong, value <= 0 ? 1 : value);
    }

    public decimal GiamGia
    {
        get => _giamGia;
        set
        {
            if (SetProperty(ref _giamGia, value < 0 ? 0 : value))
            {
                OnPropertyChanged(nameof(ThanhTien));
            }
        }
    }

    public SanPham? SelectedSanPham
    {
        get => _selectedSanPham;
        set => SetProperty(ref _selectedSanPham, value);
    }

    public KhachHang? SelectedKhachHang
    {
        get => _selectedKhachHang;
        set => SetProperty(ref _selectedKhachHang, value);
    }

    public decimal TongTien => InvoiceItems.Sum(item => item.ThanhTien);
    public decimal ThanhTien => Math.Max(0, TongTien - GiamGia);

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SearchProductCommand { get; }
    public ICommand LoadCustomersCommand { get; }
    public ICommand AddToInvoiceCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand ClearCommand { get; }

    private async Task LoadCustomersAsync()
    {
        try
        {
            KhachHangs.Clear();
            foreach (var customer in await _khachHangService.SearchAsync(null))
            {
                KhachHangs.Add(customer);
            }
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
            SelectedSanPham = await _banHangService.TimSanPhamAsync(SearchKeyword);
            StatusMessage = SelectedSanPham is null
                ? "Không tìm thấy sản phẩm."
                : $"Đã chọn {SelectedSanPham.TenSanPham}.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void AddToInvoice()
    {
        if (SelectedSanPham is null)
        {
            return;
        }

        if (!SelectedSanPham.KiemTraTonKho(SoLuong))
        {
            StatusMessage = "Số lượng tồn không đủ.";
            return;
        }

        var existing = InvoiceItems.FirstOrDefault(item => item.SanPhamId == SelectedSanPham.Id);
        if (existing is not null)
        {
            var newQuantity = existing.SoLuong + SoLuong;
            if (!SelectedSanPham.KiemTraTonKho(newQuantity))
            {
                StatusMessage = "Số lượng tồn không đủ.";
                return;
            }

            existing.SoLuong = newQuantity;
            existing.TinhThanhTien();
            RefreshInvoiceItems();
        }
        else
        {
            var detail = new ChiTietHoaDon
            {
                SanPhamId = SelectedSanPham.Id,
                MaSanPham = SelectedSanPham.MaSanPham,
                TenSanPham = SelectedSanPham.TenSanPham,
                SoLuong = SoLuong,
                DonGia = SelectedSanPham.GiaBan
            };
            detail.TinhThanhTien();
            InvoiceItems.Add(detail);
        }

        NotifyTotals();
        StatusMessage = "Đã thêm sản phẩm vào hóa đơn.";
    }

    private void RemoveItem(ChiTietHoaDon? item)
    {
        if (item is null)
        {
            return;
        }

        InvoiceItems.Remove(item);
        NotifyTotals();
    }

    private async Task CheckoutAsync()
    {
        try
        {
            var hoaDon = new HoaDon
            {
                NhanVienId = _nhanVienId,
                KhachHangId = SelectedKhachHang?.Id,
                GiamGia = GiamGia,
                ChiTietHoaDons = InvoiceItems.Select(item => new ChiTietHoaDon
                {
                    SanPhamId = item.SanPhamId,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia
                }).ToList()
            };

            await _banHangService.ThanhToanAsync(hoaDon);
            StatusMessage = $"Thanh toán thành công. {hoaDon.InHoaDon()}";
            ClearInvoice();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void ClearInvoice()
    {
        InvoiceItems.Clear();
        GiamGia = 0;
        SelectedSanPham = null;
        SearchKeyword = string.Empty;
        NotifyTotals();
    }

    private void RefreshInvoiceItems()
    {
        var snapshot = InvoiceItems.ToList();
        InvoiceItems.Clear();
        foreach (var item in snapshot)
        {
            InvoiceItems.Add(item);
        }
    }

    private void NotifyTotals()
    {
        OnPropertyChanged(nameof(TongTien));
        OnPropertyChanged(nameof(ThanhTien));
    }
}
