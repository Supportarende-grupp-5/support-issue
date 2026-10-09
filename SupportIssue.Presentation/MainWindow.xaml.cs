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
{
    private readonly ITicketHandlingRepository _ticketRepository;
    private readonly ITicketHandlingService _ticketService;
    private readonly ICustomerRepository _customerRepository;
    private readonly RegisterTicketView _registerView;

    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private TicketDetailsViewModel? _viewModel;
    private bool _updatingList;
    private bool _closed;

    public MainWindow(
        CustomerService customerService,
        RegisterTicketView registerTicketView,
        ITicketHandlingRepository ticketRepository,
        ITicketHandlingService ticketHandlingService,
        ICustomerRepository customerRepository)
    {
        _ticketRepository = ticketRepository;
        _ticketService = ticketHandlingService;
        _customerRepository = customerRepository;
        _registerView = registerTicketView;

        InitializeComponent();

        CustomerContent.Content = new CustomerView(customerService);
        TicketContent.Content = registerTicketView;

        _registerView.TicketRegistered += TicketDataChanged;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;
        await UpdateListAsync();
    }

    private async void TicketDataChanged(object? sender, EventArgs e)
    {
        await UpdateListAsync();
    }

    // Behåll om denna händelse fortfarande används i XAML.
    private async void RefreshTicketsButton_Click(
        object sender, RoutedEventArgs e)
    {
        await UpdateListAsync();
    }

    private async Task UpdateListAsync()
    {
        await _loadLock.WaitAsync();

        try
        {
            if (_closed) return;

            TicketsListBox.IsEnabled = false;

            var tickets = await _ticketRepository.GetAllTicketsAsync();

            if (_closed) return;

            var selectedId =
                (TicketsListBox.SelectedItem as SupportTicket)?.Id;

            var sortedTickets = tickets
                .OrderByDescending(ticket => ticket.CreatedAt)
                .ToList();

            _updatingList = true;

            try
            {
                TicketsListBox.ItemsSource = sortedTickets;
                TicketsListBox.SelectedItem = sortedTickets
                    .FirstOrDefault(ticket => ticket.Id == selectedId);
            }
            finally
            {
                _updatingList = false;
            }

            if (TicketsListBox.SelectedItem is null)
            {
                DetachViewModel();
                DetailsFrame.Content = null;
            }

            TicketListMessage.Text = sortedTickets.Count == 0
                ? "Inga ärenden hittades. Registrera ett ärende först."
                : "Välj ett ärende. Ändringar visas automatiskt.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            if (!_closed)
                TicketListMessage.Text = "Kunde inte uppdatera ärendelistan.";
        }
        finally
        {
            if (!_closed) TicketsListBox.IsEnabled = true;
            _loadLock.Release();
        }
    }

    private async void TicketsListBox_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        if (_closed || _updatingList ||
            TicketsListBox.SelectedItem is not SupportTicket ticket)
            return;

        await _loadLock.WaitAsync();

        try
        {
            if (_closed) return;

            TicketsListBox.IsEnabled = false;
            DetachViewModel();
            DetailsFrame.Content = null;

            var viewModel = new TicketDetailsViewModel(
                _ticketService, _customerRepository);

            await viewModel.LoadAsync(ticket.Id);

            if (_closed) return;

            DetailsFrame.Content = new TicketDetailsPage(viewModel);

            _viewModel = viewModel;
            _viewModel.TicketChanged += TicketDataChanged;

            TicketListMessage.Text = "Ändringar visas automatiskt.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            if (!_closed)
                TicketListMessage.Text = "Kunde inte öppna det valda ärendet.";
        }
        finally
        {
            if (!_closed) TicketsListBox.IsEnabled = true;
            _loadLock.Release();
        }
    }

    private void DetachViewModel()
    {
        if (_viewModel is not null)
            _viewModel.TicketChanged -= TicketDataChanged;

        _viewModel = null;
    }

    protected override void OnClosed(EventArgs e)
    {
        _closed = true;
        Loaded -= MainWindow_Loaded;
        _registerView.TicketRegistered -= TicketDataChanged;
        DetachViewModel();

        base.OnClosed(e);
    }
}