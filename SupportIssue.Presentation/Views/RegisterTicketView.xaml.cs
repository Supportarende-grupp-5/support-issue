using System.IO;
using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.Models;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Presentation.Views;

public partial class RegisterTicketView : UserControl
{
    private readonly ITicketService _ticketService;
    private bool _isSaving;

    public RegisterTicketView(ITicketService ticketService)
    {
        ArgumentNullException.ThrowIfNull(ticketService);

        InitializeComponent();

        _ticketService = ticketService;

        PriorityComboBox.ItemsSource = Enum.GetValues<TicketPriority>();
        PriorityComboBox.SelectedItem = TicketPriority.Normal;

        Loaded += RegisterTicketView_Loaded;
    }

    private void RegisterTicketView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (_isSaving)
            return;

        RegisterButton.IsEnabled = false;

        try
        {
            var selectedCustomerId = CustomerComboBox.SelectedValue;

            CustomerComboBox.ItemsSource = _ticketService.GetCustomers();
            CustomerComboBox.SelectedValue = selectedCustomerId;

            bool hasCustomers = CustomerComboBox.Items.Count > 0;

            RegisterButton.IsEnabled = hasCustomers;

            FeedbackTextBlock.Text = hasCustomers
                ? string.Empty
                : "No customers available. Register a customer first.";
        }
        catch (Exception ex) when (
            ex is InvalidOperationException ||
            ex is IOException ||
            ex is UnauthorizedAccessException)
        {
            CustomerComboBox.ItemsSource = null;

            FeedbackTextBlock.Text =
                "Customers could not be loaded. Check the customer file and access permissions.";
        }
    }

    private async void RegisterButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_isSaving)
            return;

        FeedbackTextBlock.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            FeedbackTextBlock.Text = "Title is required.";
            TitleTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            FeedbackTextBlock.Text = "Description is required.";
            DescriptionTextBox.Focus();
            return;
        }

        if (CustomerComboBox.SelectedValue is not Guid customerId ||
            customerId == Guid.Empty)
        {
            FeedbackTextBlock.Text = "Please select a customer.";
            CustomerComboBox.Focus();
            return;
        }

        if (PriorityComboBox.SelectedItem is not TicketPriority priority)
        {
            FeedbackTextBlock.Text = "Please select a priority.";
            PriorityComboBox.Focus();
            return;
        }

        var request = new CreateTicketRequest
        {
            Title = TitleTextBox.Text,
            Description = DescriptionTextBox.Text,
            CustomerId = customerId,
            Priority = priority
        };

        _isSaving = true;
        IsEnabled = false;
        FeedbackTextBlock.Text = "Saving ticket...";

        try
        {
            var ticket = await _ticketService.CreateTicketAsync(request);

            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            CustomerComboBox.SelectedIndex = -1;
            PriorityComboBox.SelectedItem = TicketPriority.Normal;

            FeedbackTextBlock.Text =
                $"Ticket registered successfully. ID: {ticket.Id}";
        }
        catch (ArgumentException ex)
        {
            FeedbackTextBlock.Text = ex.Message;
        }
        catch (InvalidDataException)
        {
            FeedbackTextBlock.Text =
                "The ticket file contains invalid data. The ticket was not saved.";
        }
        catch (IOException)
        {
            FeedbackTextBlock.Text =
                "The ticket could not be saved. Check the file and try again.";
        }
        catch (UnauthorizedAccessException)
        {
            FeedbackTextBlock.Text =
                "You do not have permission to access the ticket file.";
        }
        finally
        {
            _isSaving = false;
            IsEnabled = true;
        }
    }
}