using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SupportIssue.Application.Customers;
using SupportIssue.Infrastructure.Customers;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.Services;
using SupportIssue.Infrastructure.Repositories;
using SupportIssue.Presentation.Views;

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

        services.AddSingleton<ITicketRepository>(
    new JsonTicketRepository("tickets.json"));

        services.AddSingleton<ITicketService, TicketService>();

        services.AddTransient<RegisterTicketView>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var customerService =
            _serviceProvider.GetRequiredService<CustomerService>();

        var registerTicketView =
    _serviceProvider.GetRequiredService<RegisterTicketView>();

        MainWindow mainWindow =
            new MainWindow(customerService, registerTicketView);

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}