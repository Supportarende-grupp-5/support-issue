using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SupportIssue.Application.Customers;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.Services;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Infrastructure.Customers;
using SupportIssue.Infrastructure.Repositories;
using SupportIssue.Presentation.Views;

namespace SupportIssue.Presentation;

public partial class App : System.Windows.Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();

        // Kundlagring och kundtjänst.
        services.AddSingleton<ICustomerRepository>(
            new JsonCustomerRepository("customer.json"));

        services.AddSingleton<CustomerService>();


        // Ett gemensamt repository för båda ärendedelarna.
        services.AddSingleton<JsonTicketRepository>(
            new JsonTicketRepository("tickets.json"));

        services.AddSingleton<ITicketRepository>(provider =>
            provider.GetRequiredService<JsonTicketRepository>());

        services.AddSingleton<ITicketHandlingRepository>(provider =>
            provider.GetRequiredService<JsonTicketRepository>());

        // Tjänster för registrering och ärendehantering.
        services.AddSingleton<ITicketService, TicketService>();

        services.AddSingleton<ITicketHandlingService, TicketHandlingService>();

        // Registreringsvyn.
        services.AddTransient<RegisterTicketView>();

        _serviceProvider = services.BuildServiceProvider();
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var customerService =
            _serviceProvider.GetRequiredService<CustomerService>();

        var registerTicketView =
            _serviceProvider.GetRequiredService<RegisterTicketView>();

        var ticketRepository =
            _serviceProvider.GetRequiredService<ITicketHandlingRepository>();

        var ticketHandlingService =
            _serviceProvider.GetRequiredService<ITicketHandlingService>();

        var customerRepository =
            _serviceProvider.GetRequiredService<ICustomerRepository>();

        var mainWindow = new MainWindow(
            customerService,
            registerTicketView,
            ticketRepository,
            ticketHandlingService,
            customerRepository);

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}