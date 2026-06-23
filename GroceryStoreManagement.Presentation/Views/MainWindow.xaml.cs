using System.Windows;
using GroceryStoreManagement.Presentation.ViewModels;

namespace GroceryStoreManagement.Presentation.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
