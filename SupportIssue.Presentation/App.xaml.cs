using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SupportIssue.Application.Customers;
using SupportIssue.Infrastructure.Customers;

namespace SupportIssue.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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

            CustomerService customerService = _serviceProvider.GetRequiredService<CustomerService>();

            MainWindow mainWindow = new MainWindow(customerService);
            mainWindow.Show();
        }
    }

}
