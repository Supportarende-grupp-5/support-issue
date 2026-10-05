using SupportIssue.Infrastructure.TicketHandling;
using SupportIssue.Presentation.ViewModels;
using SupportIssue.Presentation.Views;
using System.Windows;

namespace SupportIssue.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;
        try
        {
            var preview = await TicketHandlingPreview.CreateAsync();
            var viewModel = new TicketDetailsViewModel(preview.Ticket, preview.Service);
            DetailsFrame.Content = new TicketDetailsPage(viewModel);
        }
        catch (Exception)
        {
            MessageBox.Show("Förhandsvisningen kunde inte öppnas.");
        }
    }
}
