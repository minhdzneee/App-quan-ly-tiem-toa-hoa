using System.Windows;
using System.Windows.Controls;
using GroceryStoreManagement.Presentation.ViewModels;

namespace GroceryStoreManagement.Presentation.Views;

public partial class RegisterView : Window
{
    public RegisterView()
    {
        InitializeComponent();

        var viewModel = new RegisterViewModel(App.Services.AuthService);
        viewModel.RegisterSucceeded += () =>
        {
            DialogResult = true;
            Close();
        };
        DataContext = viewModel;
    }

    private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.MatKhau = passwordBox.Password;
        }
    }

    private void ConfirmPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.XacNhanMatKhau = passwordBox.Password;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
