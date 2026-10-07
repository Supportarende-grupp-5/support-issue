using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.Customers;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain.Entities;
using SupportIssue.Presentation.ViewModels;
using SupportIssue.Presentation.Views;

namespace SupportIssue.Presentation;

public partial class MainWindow : Window
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SupportIssue.Application.Customers;

namespace SupportIssue.Presentation
{
    private readonly ITicketHandlingRepository _ticketRepository;
    private readonly ITicketHandlingService _ticketHandlingService;
    private readonly ICustomerRepository _customerRepository;

    private bool _isLoading;
    private bool _isUpdatingList;

    public MainWindow(
        CustomerService customerService,
        RegisterTicketView registerTicketView,
        ITicketHandlingRepository ticketRepository,
        ITicketHandlingService ticketHandlingService,
        ICustomerRepository customerRepository)
    {
        _ticketRepository = ticketRepository;
        _ticketHandlingService = ticketHandlingService;
        _customerRepository = customerRepository;

        InitializeComponent();

        CustomerContent.Content = new CustomerView(customerService);
        TicketContent.Content = registerTicketView;

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;

        await LoadTicketsAsync();
    }

    private async void RefreshTicketsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        await LoadTicketsAsync();
    }

    private async Task LoadTicketsAsync()
    {
        if (_isLoading)
        {
            return;
        }

        SetLoading(true);
        TicketListMessage.Text = "Loading tickets...";

        try
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();

            // Undvik att öppna ett ärende medan listan byts ut.
            _isUpdatingList = true;

            try
            {
                TicketsListBox.ItemsSource = tickets
                    .OrderByDescending(ticket => ticket.CreatedAt)
                    .ToList();

                TicketsListBox.SelectedItem = null;
                DetailsFrame.Content = null;
            }
            finally
            {
                _isUpdatingList = false;
            }

            TicketListMessage.Text = tickets.Count == 0
                ? "No tickets found. Register a ticket first."
                : "Select a ticket to view its details.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            TicketListMessage.Text =
                "Could not load tickets. Check the ticket file and try again.";
        }
        finally
        {
            SetLoading(false);
        }
    }

    private async void TicketsListBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_isUpdatingList || _isLoading)
        {
            return;
        }

        if (TicketsListBox.SelectedItem is not SupportTicket ticket)
        {
            return;
        private readonly CustomerService _customerService;
        public MainWindow(CustomerService customerService)
        {
            InitializeComponent();

            _customerService = customerService;
        }

        SetLoading(true);
        DetailsFrame.Content = null;
        TicketListMessage.Text = "Opening ticket...";

        try
        {
            var viewModel = new TicketDetailsViewModel(
                _ticketHandlingService,
                _customerRepository);

            await viewModel.LoadAsync(ticket.Id);

            DetailsFrame.Content = new TicketDetailsPage(viewModel);

            TicketListMessage.Text =
                "Refresh the list after registering or renaming a ticket.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            TicketListMessage.Text =
                "Could not open the selected ticket.";
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void SetLoading(bool isLoading)
    {
        _isLoading = isLoading;

        RefreshTicketsButton.IsEnabled = !isLoading;
        TicketsListBox.IsEnabled = !isLoading;
    }
}