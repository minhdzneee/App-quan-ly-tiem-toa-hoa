using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private string _tenDangNhap = "admin";
    private string _matKhau = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
        LoginCommand = new AsyncRelayCommand(_ => LoginAsync());
        OpenRegisterCommand = new RelayCommand(_ => RegisterRequested?.Invoke());
    }

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

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand OpenRegisterCommand { get; }

    public event Action<TaiKhoan>? LoginSucceeded;
    public event Action? RegisterRequested;

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            var taiKhoan = await _authService.LoginAsync(TenDangNhap, MatKhau);
            LoginSucceeded?.Invoke(taiKhoan);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
