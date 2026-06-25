using System.Windows;
using System.Windows.Controls;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.ViewModels;

namespace GroceryStoreManagement.Presentation.Views;

public partial class ChangePasswordView : Window
{
    public ChangePasswordView(AuthService authService, TaiKhoan currentUser)
    {
        InitializeComponent();

        var viewModel = new ChangePasswordViewModel(authService, currentUser);
        viewModel.ChangePasswordSucceeded += () =>
        {
            DialogResult = true;
            Close();
        };
        DataContext = viewModel;
    }

    private void OldPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.MatKhauCu = passwordBox.Password;
        }
    }

    private void NewPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.MatKhauMoi = passwordBox.Password;
        }
    }

    private void ConfirmPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.XacNhanMatKhau = passwordBox.Password;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
