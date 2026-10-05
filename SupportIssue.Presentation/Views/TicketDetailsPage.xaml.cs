using SupportIssue.Application.TicketHandling;
using SupportIssue.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SupportIssue.Presentation.Views;

public partial class TicketDetailsPage : Page
{
    public TicketDetailsPage(TicketDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private async void AssignTechnicianButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TicketDetailsViewModel viewModel) return;
        AddCommentButton.IsEnabled = false;
        AssignTechnicianButton.IsEnabled = false;
        AssignPriorityButton.IsEnabled = false;
        try
        {
            if (await viewModel.AssignSelectedTechnicianAsync())
                MessageBox.Show("Handläggaren har tilldelats.");
            else
                MessageBox.Show("Handläggaren kunde inte tilldelas.");
        }
        catch (ArgumentException)
        {
            MessageBox.Show("Välj en giltig handläggare.");
        }
        catch (Exception)
        {
            MessageBox.Show("Handläggaren kunde inte tilldelas. Kontrollera datafilen och försök igen.");
        }
        finally
        {
            AddCommentButton.IsEnabled = true;
            AssignTechnicianButton.IsEnabled = true;
            AssignPriorityButton.IsEnabled = true;
        }
    }
    private async void AssignPriorityButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not TicketDetailsViewModel viewModel) return;
        AddCommentButton.IsEnabled = false;
        AssignTechnicianButton.IsEnabled = false;
        AssignPriorityButton.IsEnabled = false;
        try
        {
            if (await viewModel.ChangePriorityAsync(viewModel.SelectedPriority))
                MessageBox.Show("Prioriteten har tilldelats.");
            else
                MessageBox.Show("Prioriteten kunde inte tilldelas.");
        }
        catch (ArgumentException)
        {
            MessageBox.Show("Välj en giltig prioritet.");
        }
        catch (Exception)
        {
            MessageBox.Show("Prioriteten kunde inte tilldelas. Kontrollera datafilen och försök igen.");
        }
        finally
        {
            AddCommentButton.IsEnabled = true;
            AssignTechnicianButton.IsEnabled = true;
            AssignPriorityButton.IsEnabled = true;

        }
    }
}
