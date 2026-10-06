using System.Windows;
using SupportIssue.Application.Customers;
using SupportIssue.Presentation.Views;

namespace SupportIssue.Presentation;

public partial class MainWindow : Window
{
    public MainWindow(
        CustomerService customerService,
        RegisterTicketView registerTicketView)
    {
        InitializeComponent();

        CustomerContent.Content = new CustomerView(customerService);
        TicketContent.Content = registerTicketView;
    }
}