using System.Windows;
using System.Windows.Controls;
using GroceryStoreManagement.Presentation.ViewModels;

namespace GroceryStoreManagement.Presentation.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();

        var viewModel = new LoginViewModel(App.Services.AuthService);
        viewModel.LoginSucceeded += account =>
        {
            var mainWindow = new MainWindow(new MainViewModel(App.Services, account));
            mainWindow.Show();
            Close();
        };
        viewModel.RegisterRequested += () =>
        {
            var registerView = new RegisterView
            {
                Owner = this
            };
            registerView.ShowDialog();
        };

        DataContext = viewModel;
    }

    private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.MatKhau = passwordBox.Password;
        }
    }
}
