using System.Windows.Input;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly AppServices _services;
    private BaseViewModel? _currentViewModel;
    private string _pageTitle = "Dashboard";

    public MainViewModel(AppServices services, TaiKhoan currentUser)
    {
        _services = services;
        CurrentUser = currentUser;

        ShowDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
        ShowSanPhamCommand = new RelayCommand(_ => NavigateToSanPham());
        ShowNhapHangCommand = new RelayCommand(_ => NavigateToNhapHang());
        ShowBanHangCommand = new RelayCommand(_ => NavigateToBanHang());
        ShowKhachHangCommand = new RelayCommand(_ => NavigateToKhachHang());
        ShowKiemKeCommand = new RelayCommand(_ => NavigateToKiemKe());
        ShowBaoCaoCommand = new RelayCommand(_ => NavigateToBaoCao());

        NavigateToDashboard();
    }

    public TaiKhoan CurrentUser { get; }

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string PageTitle
    {
        get => _pageTitle;
        set => SetProperty(ref _pageTitle, value);
    }

    public string UserDisplayName => CurrentUser.HoTenNhanVien ?? CurrentUser.TenDangNhap;
    public string RoleDisplayName => CurrentUser.TenVaiTro;

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowSanPhamCommand { get; }
    public ICommand ShowNhapHangCommand { get; }
    public ICommand ShowBanHangCommand { get; }
    public ICommand ShowKhachHangCommand { get; }
    public ICommand ShowKiemKeCommand { get; }
    public ICommand ShowBaoCaoCommand { get; }

    private void NavigateToDashboard()
    {
        PageTitle = "Dashboard";
        CurrentViewModel = new DashboardViewModel(_services.BaoCaoService);
    }

    private void NavigateToSanPham()
    {
        PageTitle = "Quản lý sản phẩm";
        CurrentViewModel = new SanPhamViewModel(_services.SanPhamService, _services.DanhMucService, _services.DonViTinhService);
    }

    private void NavigateToNhapHang()
    {
        PageTitle = "Nhập hàng";
        CurrentViewModel = new NhapHangViewModel(_services.NhapHangService, _services.NhaCungCapService, _services.SanPhamService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToBanHang()
    {
        PageTitle = "Bán hàng";
        CurrentViewModel = new BanHangViewModel(_services.BanHangService, _services.KhachHangService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToKhachHang()
    {
        PageTitle = "Khách hàng";
        CurrentViewModel = new KhachHangViewModel(_services.KhachHangService);
    }

    private void NavigateToKiemKe()
    {
        PageTitle = "Kiểm kê kho";
        CurrentViewModel = new KiemKeViewModel(_services.KiemKeService, _services.SanPhamService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToBaoCao()
    {
        PageTitle = "Báo cáo thống kê";
        CurrentViewModel = new BaoCaoViewModel(_services.BaoCaoService);
    }
}
