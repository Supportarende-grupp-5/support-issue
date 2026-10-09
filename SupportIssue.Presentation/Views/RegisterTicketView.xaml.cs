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

        PriorityComboBox.ItemsSource = new[]
{
    new { Text = "Låg", Value = TicketPriority.Low },
    new { Text = "Normal", Value = TicketPriority.Normal },
    new { Text = "Hög", Value = TicketPriority.High }
};

        PriorityComboBox.DisplayMemberPath = "Text";
        PriorityComboBox.SelectedValuePath = "Value";
        PriorityComboBox.SelectedValue = TicketPriority.Normal;

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
                : "Det finns inga registrerade kunder. Registrera en kund först..";
        }
        catch (Exception ex) when (
            ex is InvalidOperationException ||
            ex is IOException ||
            ex is UnauthorizedAccessException)
        {
            CustomerComboBox.ItemsSource = null;

            FeedbackTextBlock.Text =
                "Kunderna kunde inte läsas in. Kontrollera kundfilen och behörigheterna.";
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
            FeedbackTextBlock.Text = "Rubrik är obligatorisk.";
            TitleTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            FeedbackTextBlock.Text = "Beskrivning är obligatorisk.";
            DescriptionTextBox.Focus();
            return;
        }

        if (CustomerComboBox.SelectedValue is not Guid customerId ||
            customerId == Guid.Empty)
        {
            FeedbackTextBlock.Text = "Välj en kund.";
            CustomerComboBox.Focus();
            return;
        }

        if (PriorityComboBox.SelectedValue is not TicketPriority priority)
        {
            FeedbackTextBlock.Text = "Välj en prioritet.";
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
        FeedbackTextBlock.Text = "Sparar ärende...";

        try
        {
            var ticket = await _ticketService.CreateTicketAsync(request);

            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            CustomerComboBox.SelectedIndex = -1;
            PriorityComboBox.SelectedValue = TicketPriority.Normal;

            FeedbackTextBlock.Text =
                $"Ärende registrerat successfully. ID: {ticket.Id}";
        }
        catch (ArgumentException ex)
        {
            FeedbackTextBlock.Text = ex.Message;
        }
        catch (InvalidDataException)
        {
            FeedbackTextBlock.Text =
                "Ärendefilen innehåller ogiltig information. Ärendet sparades inte..";
        }
        catch (IOException)
        {
            FeedbackTextBlock.Text =
                "Ärendet kunde inte sparas. Kontrollera filen och försök igen.";
        }
        catch (UnauthorizedAccessException)
        {
            FeedbackTextBlock.Text =
                "Du har inte tillstånd att komma åt ärendefilen.";
        }
        finally
        {
            _isSaving = false;
            IsEnabled = true;
        }
    }
}