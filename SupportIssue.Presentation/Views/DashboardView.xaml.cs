using System.Windows;
using System.Windows.Controls;
using SupportIssue.Application.TicketOverview;

namespace SupportIssue.Presentation.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly TicketOverviewService _overviewService;

        public DashboardView(TicketOverviewService overviewService)
        {
            InitializeComponent();

            _overviewService = overviewService;
        }

        // Körs varje gång användaren öppnar fliken, så siffrorna alltid är aktuella
        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCountsAsync();
        }

        private async Task LoadCountsAsync()
        {
            try
            {
                var counts = await _overviewService.GetStatusCountsAsync();

                TotalCountText.Text = counts.TotalCount.ToString();
                NewCountText.Text = counts.NewCount.ToString();
                InProgressCountText.Text = counts.InProgressCount.ToString();
                ResolvedCountText.Text = counts.ResolvedCount.ToString();

                MessageText.Text = "Uppdaterad " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageText.Text = "Översikten kunde inte läsas in: " + ex.Message;
            }
        }
    }
}
