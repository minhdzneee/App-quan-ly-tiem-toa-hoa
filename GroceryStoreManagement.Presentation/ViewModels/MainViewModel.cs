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
        LogoutCommand = new AsyncRelayCommand(_ => LogoutAsync());

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

    public bool CanViewDashboard => true;
    public bool CanViewSanPham => IsAdmin || IsQuanLy || IsNhanVienKho;
    public bool CanViewNhapHang => IsAdmin || IsQuanLy || IsNhanVienKho;
    public bool CanViewBanHang => IsAdmin || IsQuanLy || IsNhanVienBanHang;
    public bool CanViewKhachHang => IsAdmin || IsQuanLy || IsNhanVienBanHang;
    public bool CanViewKiemKe => IsAdmin || IsQuanLy || IsNhanVienKho;
    public bool CanViewBaoCao => IsAdmin || IsQuanLy;

    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowSanPhamCommand { get; }
    public ICommand ShowNhapHangCommand { get; }
    public ICommand ShowBanHangCommand { get; }
    public ICommand ShowKhachHangCommand { get; }
    public ICommand ShowKiemKeCommand { get; }
    public ICommand ShowBaoCaoCommand { get; }
    public ICommand LogoutCommand { get; }

    public event Action? LogoutRequested;

    private bool IsAdmin => IsInRole("Admin");
    private bool IsQuanLy => IsInRole("QuanLy");
    private bool IsNhanVienKho => IsInRole("NhanVienKho");
    private bool IsNhanVienBanHang => IsInRole("NhanVienBanHang");

    private void NavigateToDashboard()
    {
        PageTitle = "Dashboard";
        CurrentViewModel = new DashboardViewModel(_services.BaoCaoService);
    }

    private void NavigateToSanPham()
    {
        if (!CanViewSanPham)
        {
            return;
        }

        PageTitle = "Quản lý sản phẩm";
        CurrentViewModel = new SanPhamViewModel(_services.SanPhamService, _services.DanhMucService, _services.DonViTinhService);
    }

    private void NavigateToNhapHang()
    {
        if (!CanViewNhapHang)
        {
            return;
        }

        PageTitle = "Nhập hàng";
        CurrentViewModel = new NhapHangViewModel(_services.NhapHangService, _services.NhaCungCapService, _services.SanPhamService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToBanHang()
    {
        if (!CanViewBanHang)
        {
            return;
        }

        PageTitle = "Bán hàng";
        CurrentViewModel = new BanHangViewModel(_services.BanHangService, _services.KhachHangService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToKhachHang()
    {
        if (!CanViewKhachHang)
        {
            return;
        }

        PageTitle = "Khách hàng";
        CurrentViewModel = new KhachHangViewModel(_services.KhachHangService);
    }

    private void NavigateToKiemKe()
    {
        if (!CanViewKiemKe)
        {
            return;
        }

        PageTitle = "Kiểm kê kho";
        CurrentViewModel = new KiemKeViewModel(_services.KiemKeService, _services.SanPhamService, CurrentUser.NhanVienId ?? 1);
    }

    private void NavigateToBaoCao()
    {
        if (!CanViewBaoCao)
        {
            return;
        }

        PageTitle = "Báo cáo thống kê";
        CurrentViewModel = new BaoCaoViewModel(_services.BaoCaoService);
    }

    private bool IsInRole(string role)
    {
        return string.Equals(CurrentUser.TenVaiTro, role, StringComparison.OrdinalIgnoreCase);
    }

    private async Task LogoutAsync()
    {
        try
        {
            await _services.AuthService.LogoutAsync(CurrentUser.Id);
        }
        finally
        {
            LogoutRequested?.Invoke();
        }
    }
}
