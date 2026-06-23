using System.Windows;

namespace GroceryStoreManagement.Presentation;

public partial class App : Application
{
    public const string DefaultConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=GroceryStoreManagement;Trusted_Connection=True;TrustServerCertificate=True;";

    public static AppServices Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Services = new AppServices(DefaultConnectionString);
        base.OnStartup(e);
    }
}
