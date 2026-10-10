using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SupportIssue.Application.TicketOverview;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Presentation.Views
{
    /// <summary>
    /// Interaction logic for TicketsView.xaml
    /// </summary>
    public partial class TicketsView : UserControl
    {
        private readonly TicketOverviewService _overviewService;

        public TicketsView(TicketOverviewService overviewService)
        {
            InitializeComponent();

            _overviewService = overviewService;
        }

        // Körs varje gång användaren öppnar fliken
        private async void TicketsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTicketsAsync();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTicketsAsync();
        }

        private async void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoadTicketsAsync();
            }
        }

        private async void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Händelsen körs också medan vyn byggs. Då är tjänsten inte satt än.
            if (!IsLoaded)
            {
                return;
            }

            await LoadTicketsAsync();
        }

        private async void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            StatusComboBox.SelectedIndex = 0;

            await LoadTicketsAsync();
        }

        private async Task LoadTicketsAsync()
        {
            string searchText = SearchTextBox.Text;
            TicketStatus? status = GetSelectedStatus();

            try
            {
                var tickets = await _overviewService.GetTicketsAsync(searchText, status);

                TicketsDataGrid.ItemsSource = tickets;

                if (tickets.Count == 0)
                {
                    MessageText.Text = "Inga ärenden hittades.";
                }
                else
                {
                    MessageText.Text = tickets.Count + " ärenden hittades.";
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = "Ärendena kunde inte läsas in: " + ex.Message;
            }
        }

        private TicketStatus? GetSelectedStatus()
        {
            switch (StatusComboBox.SelectedIndex)
            {
                case 1:
                    return TicketStatus.New;
                case 2:
                    return TicketStatus.InProgress;
                case 3:
                    return TicketStatus.Resolved;
                default:
                    return null; // "Alla"
            }
        }
    }
}
