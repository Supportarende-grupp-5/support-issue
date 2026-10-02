using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using SupportIssue.Infrastructure.TicketHandling;
using SupportIssue.Presentation.ViewModels;
using SupportIssue.Presentation.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SupportIssue.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Tillfällig förhandsvisning med exempeldata i minnet.
            var ticket = new SupportTicket(
                "Skrivaren fungerar inte",
                "Kontorets skrivare tar emot utskrifter men skriver inte ut några sidor.",
                Guid.NewGuid(),
                TicketPriority.Normal);

            ticket.AssignTechnician(new Technician("Anna", 1));
            ticket.UpdateStatus(TicketStatus.InProgress);
            ticket.AddComment("Kontrollerat att skrivaren är ansluten till nätverket.");
            ticket.AddComment("Startat om skrivaren och skickat en provutskrift.");

            var repository = new JsonFileTicketHandlingRepository();
            var service = new TicketHandlingService(repository);
            var viewModel = new TicketDetailsViewModel(ticket, service);
            DetailsFrame.Content = new TicketDetailsPage(viewModel);
        }
    }
}
