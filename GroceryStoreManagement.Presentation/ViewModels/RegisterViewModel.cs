using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private string _tenDangNhap = string.Empty;
    private string _matKhau = string.Empty;
    private string _xacNhanMatKhau = string.Empty;
    private string _hoTen = string.Empty;
    private VaiTro? _selectedVaiTro;
    private string _statusMessage = string.Empty;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService;
        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        RegisterCommand = new AsyncRelayCommand(_ => RegisterAsync());
        LoadCommand.Execute(null);
    }

    public ObservableCollection<VaiTro> VaiTros { get; } = new();

    public string TenDangNhap
    {
        get => _tenDangNhap;
        set => SetProperty(ref _tenDangNhap, value);
    }

    public string MatKhau
    {
        get => _matKhau;
        set => SetProperty(ref _matKhau, value);
    }

    public string XacNhanMatKhau
    {
        get => _xacNhanMatKhau;
        set => SetProperty(ref _xacNhanMatKhau, value);
    }

    public string HoTen
    {
        get => _hoTen;
        set => SetProperty(ref _hoTen, value);
    }

    public VaiTro? SelectedVaiTro
    {
        get => _selectedVaiTro;
        set => SetProperty(ref _selectedVaiTro, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand RegisterCommand { get; }

    public event Action? RegisterSucceeded;

    private async Task LoadAsync()
    {
        try
        {
            VaiTros.Clear();
            foreach (var role in await _authService.GetVaiTrosAsync())
            {
                VaiTros.Add(role);
            }

            SelectedVaiTro = VaiTros.FirstOrDefault(role => role.TenVaiTro == "NhanVienBanHang")
                ?? VaiTros.FirstOrDefault();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task RegisterAsync()
    {
        try
        {
            await _authService.RegisterAsync(TenDangNhap, MatKhau, XacNhanMatKhau, SelectedVaiTro?.Id ?? 0, HoTen);
            StatusMessage = "Tạo tài khoản thành công.";
            RegisterSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
