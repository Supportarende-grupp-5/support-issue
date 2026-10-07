using SupportIssue.Presentation.ViewModels;
using System.Windows.Controls;

namespace SupportIssue.Presentation.Views;

public partial class TicketDetailsPage : Page
{
    public TicketDetailsViewModel ViewModel { get; }

    public TicketDetailsPage(TicketDetailsViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
    }
}
