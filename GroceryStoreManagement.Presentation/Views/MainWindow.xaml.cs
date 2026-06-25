using System.Windows;
using GroceryStoreManagement.Presentation.ViewModels;

namespace GroceryStoreManagement.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        viewModel.LogoutRequested += OnLogoutRequested;
        viewModel.ChangePasswordRequested += () =>
        {
            var changePasswordView = new ChangePasswordView(App.Services.AuthService, viewModel.CurrentUser)
            {
                Owner = this
            };
            changePasswordView.ShowDialog();
        };
        DataContext = viewModel;
    }

    private void OnLogoutRequested()
    {
        var loginView = new LoginView();
        loginView.Show();
        Close();
    }
}
