using SupportIssue.Presentation.ViewModels;
using System.Windows.Controls;
namespace SupportIssue.Presentation.Views;
public partial class TicketDetailsPage : Page
{
    public TicketDetailsPage(TicketDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
