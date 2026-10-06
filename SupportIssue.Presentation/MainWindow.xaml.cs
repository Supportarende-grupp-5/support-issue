using System.Windows;
using SupportIssue.Application.Customers;
using SupportIssue.Presentation.Views;

namespace SupportIssue.Presentation;

public partial class MainWindow : Window
{
    private readonly CustomerService _customerService;

    public MainWindow(
        CustomerService customerService,
        RegisterTicketView registerTicketView)
    {
        InitializeComponent();

        _customerService = customerService;
        MainContent.Content = registerTicketView;
    }
}