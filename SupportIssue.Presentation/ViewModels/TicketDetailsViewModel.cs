using SupportIssue.Application.TicketHandling;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SupportIssue.Presentation.ViewModels;

public partial class TicketDetailsViewModel : ObservableObject
{
    private TicketDetails currentTicket;
    private readonly ITicketHandlingService ticketHandlingService;
    public string Title => currentTicket.Title;
    public string Description => currentTicket.Description;
    public TicketStatusOption Status => currentTicket.Status;
    public TicketPriorityOption Priority => currentTicket.Priority;
    public string TechnicianName => currentTicket.TechnicianName ?? "Ej tilldelad";
    public ObservableCollection<TicketCommentDetails> Comments { get; }
    public IReadOnlyList<TechnicianOption> Technicians { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommentCommand))]
    public partial string NewCommentText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyCanExecuteChangedFor(nameof(AddCommentCommand))]
    [NotifyCanExecuteChangedFor(nameof(AssignSelectedTechnicianCommand))]
    [NotifyCanExecuteChangedFor(nameof(ChangePriorityCommand))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AssignSelectedTechnicianCommand))]
    public partial TechnicianOption? SelectedTechnician { get; set; }

    public TicketDetailsViewModel(TicketDetails ticket, ITicketHandlingService service)
    {
        currentTicket = ticket;
        ticketHandlingService = service;
        Technicians = service.GetTechnicians();
        Comments = new ObservableCollection<TicketCommentDetails>(ticket.Comments);
        SelectedTechnician = Technicians.FirstOrDefault(item => item.TechnicianId == ticket.AssignedTechnicianId);
        SelectedPriority = ticket.Priority;
    }

    private bool CanAddComment() => !IsBusy && !string.IsNullOrWhiteSpace(NewCommentText);

    [RelayCommand(CanExecute = nameof(CanAddComment))]
    private async Task AddCommentAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.AddCommentToTicket(currentTicket.Id, NewCommentText);
            if (!result)
            {
                StatusMessage = "Kommentaren kunde inte sparas.";
                return;
            }
            await RefreshAsync();
            NewCommentText = string.Empty;
            StatusMessage = "Kommentaren har lagts till.";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Skriv en kommentar som inte är tom eller bara innehåller blanksteg.";
        }
        catch (Exception)
        {
            StatusMessage = "Kommentaren kunde inte läggas till. Kontrollera datafilen och försök igen.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAssignTechnician() => !IsBusy && SelectedTechnician != null;

    [RelayCommand(CanExecute = nameof(CanAssignTechnician))]
    private async Task AssignSelectedTechnicianAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            if (SelectedTechnician == null) throw new ArgumentException("Välj en handläggare.");
            var result = await ticketHandlingService.AssignTechnician(currentTicket.Id, SelectedTechnician.TechnicianId);
            if (!result)
            {
                StatusMessage = "Handläggaren kunde inte tilldelas.";
                return;
            }
            await RefreshAsync();
            StatusMessage = "Handläggaren har tilldelats.";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Välj en giltig handläggare.";
        }
        catch (Exception)
        {
            StatusMessage = "Handläggaren kunde inte tilldelas. Kontrollera datafilen och försök igen.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanChangePriority() => !IsBusy && Enum.IsDefined(SelectedPriority);

    [RelayCommand(CanExecute = nameof(CanChangePriority))]
    private async Task ChangePriorityAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.ChangeTicketPriority(currentTicket.Id, SelectedPriority);
            if (!result)
            {
                StatusMessage = "Prioriteten kunde inte ändras.";
                return;
            }
            await RefreshAsync();
            StatusMessage = "Prioriteten har ändrats.";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Välj en giltig prioritet.";
        }
        catch (Exception)
        {
            StatusMessage = "Prioriteten kunde inte ändras. Kontrollera datafilen och försök igen.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePriorityCommand))]
    public partial TicketPriorityOption SelectedPriority { get; set; }
    public IReadOnlyList<TicketPriorityOption> PriorityOptions { get; }
    = Enum.GetValues<TicketPriorityOption>();

    private async Task RefreshAsync()
    {
        currentTicket = await ticketHandlingService.GetTicketById(currentTicket.Id);
        Comments.Clear();
        foreach (var comment in currentTicket.Comments) Comments.Add(comment);
        SelectedTechnician = Technicians.FirstOrDefault(item => item.TechnicianId == currentTicket.AssignedTechnicianId);
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(TechnicianName));
        SelectedPriority = currentTicket.Priority;
    }

}
