using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.Interfaces;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Presentation.Views;

public partial class RegisterTicketView : UserControl
{
    private readonly ITicketService _ticketService;

    public RegisterTicketView(ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(ticketService);

        InitializeComponent();

        _ticketService = ticketService;

        PriorityComboBox.ItemsSource =
            Enum.GetValues<TicketPriority>();

        PriorityComboBox.SelectedItem = TicketPriority.Normal;

        Loaded += RegisterTicketView_Loaded;
    }

    private void RegisterTicketView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            CustomerComboBox.ItemsSource =
                _ticketService.GetCustomers();

            FeedbackTextBlock.Text =
                CustomerComboBox.Items.Count == 0
                    ? "No customers available. Register a customer first."
                    : string.Empty;
        }
        catch (Exception ex) when (
            ex is InvalidOperationException ||
            ex is System.IO.IOException ||
            ex is UnauthorizedAccessException)
        {
            CustomerComboBox.ItemsSource = null;

            FeedbackTextBlock.Text =
                "Customers could not be loaded. Check the customer file and access permissions.";
        }
    }
}
