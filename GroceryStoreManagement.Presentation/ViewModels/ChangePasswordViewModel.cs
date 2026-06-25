using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class ChangePasswordViewModel : BaseViewModel
{
    private readonly AuthService _authService;
    private readonly TaiKhoan _currentUser;
    private string _matKhauCu = string.Empty;
    private string _matKhauMoi = string.Empty;
    private string _xacNhanMatKhau = string.Empty;
    private string _statusMessage = string.Empty;

    public ChangePasswordViewModel(AuthService authService, TaiKhoan currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
        ChangePasswordCommand = new AsyncRelayCommand(_ => ChangePasswordAsync());
    }

    public string MatKhauCu
    {
        get => _matKhauCu;
        set => SetProperty(ref _matKhauCu, value);
    }

    public string MatKhauMoi
    {
        get => _matKhauMoi;
        set => SetProperty(ref _matKhauMoi, value);
    }

    public string XacNhanMatKhau
    {
        get => _xacNhanMatKhau;
        set => SetProperty(ref _xacNhanMatKhau, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand ChangePasswordCommand { get; }

    public event Action? ChangePasswordSucceeded;

    private async Task ChangePasswordAsync()
    {
        try
        {
            if (MatKhauMoi != XacNhanMatKhau)
            {
                throw new InvalidOperationException("Xac nhan mat khau khong khop.");
            }

            await _authService.DoiMatKhauAsync(_currentUser.Id, MatKhauCu, MatKhauMoi);
            StatusMessage = "Đổi mật khẩu thành công.";
            ChangePasswordSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}
