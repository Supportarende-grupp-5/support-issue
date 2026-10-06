using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SupportIssue.Application.Customers;
using SupportIssue.Infrastructure.Customers;

namespace SupportIssue.Presentation;

public partial class App : System.Windows.Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();

        string customerFilePath = "customer.json";

        services.AddSingleton<ICustomerRepository>(
            new JsonCustomerRepository(customerFilePath));

        services.AddSingleton<CustomerService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var customerService =
            _serviceProvider.GetRequiredService<CustomerService>();

        var mainWindow = new MainWindow(customerService);

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}