using SupportIssue.Application.TicketHandling;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SupportIssue.Presentation.ViewModels;

public class TicketDetailsViewModel : INotifyPropertyChanged
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

    private string newCommentText = string.Empty;
    public string NewCommentText
    {
        get => newCommentText;
        set
        {
            if (newCommentText == value) return;
            newCommentText = value;
            OnPropertyChanged();
        }
    }

    private TechnicianOption? selectedTechnician;
    public TechnicianOption? SelectedTechnician
    {
        get => selectedTechnician;
        set
        {
            if (selectedTechnician == value) return;
            selectedTechnician = value;
            OnPropertyChanged();
        }
    }

    public TicketDetailsViewModel(TicketDetails ticket, ITicketHandlingService service)
    {
        currentTicket = ticket;
        ticketHandlingService = service;
        Technicians = service.GetTechnicians();
        Comments = new ObservableCollection<TicketCommentDetails>(ticket.Comments);
        SelectedTechnician = Technicians.FirstOrDefault(item => item.TechnicianId == ticket.AssignedTechnicianId);
        SelectedPriority = ticket.Priority;
    }

    public async Task<bool> AddCommentAsync()
    {
        var result = await ticketHandlingService.AddCommentToTicket(currentTicket.Id, NewCommentText);
        if (result)
        {
            await RefreshAsync();
            NewCommentText = string.Empty;
        }
        return result;
    }

    public async Task<bool> AssignSelectedTechnicianAsync()
    {
        if (SelectedTechnician == null) throw new ArgumentException("Välj en handläggare.");
        var result = await ticketHandlingService.AssignTechnician(currentTicket.Id, SelectedTechnician.TechnicianId);
        if (result) await RefreshAsync();
        return result;
    }
    public async Task<bool> ChangePriorityAsync(TicketPriorityOption newPriority)
    {
        var result = await ticketHandlingService.ChangeTicketPriority(currentTicket.Id, newPriority);
        if (result) await RefreshAsync();
        return result;
    }
    private TicketPriorityOption selectedPriority;

    public TicketPriorityOption SelectedPriority
    {
        get => selectedPriority;
        set
        {
            if (selectedPriority == value) return;
            selectedPriority = value;
            OnPropertyChanged();
        }
    }
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

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
