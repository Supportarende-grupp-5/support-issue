using SupportIssue.Application.TicketHandling;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace SupportIssue.Presentation.ViewModels;

public partial class TicketDetailsViewModel : ObservableObject
{
    private TicketDetails? currentTicket;
    private readonly ITicketHandlingService ticketHandlingService;
    public string Title => currentTicket?.Title ?? string.Empty;
    public string Description => currentTicket?.Description ?? string.Empty;
    public TicketStatusOption Status => currentTicket?.Status ?? TicketStatusOption.New;
    public TicketPriorityOption Priority => currentTicket?.Priority ?? TicketPriorityOption.Normal;
    public string TechnicianName => currentTicket?.TechnicianName ?? "Ej tilldelad";
    private Guid CurrentTicketId => currentTicket?.Id
        ?? throw new InvalidOperationException("Inget ärende har lästs in.");
    public ObservableCollection<TicketCommentDetails> Comments { get; } = [];
    public IReadOnlyList<TechnicianOption> Technicians { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommentCommand))]
    public partial string NewCommentText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateTitleCommand))]
    public partial string UpdateTitleText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateDescriptionCommand))]
    public partial string UpdateDescriptionText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyCanExecuteChangedFor(nameof(AddCommentCommand))]
    [NotifyCanExecuteChangedFor(nameof(AssignSelectedTechnicianCommand))]
    [NotifyCanExecuteChangedFor(nameof(ChangePriorityCommand))]
    [NotifyCanExecuteChangedFor(nameof(CloseTicketCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateTitleCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateDescriptionCommand))]
    public partial bool IsBusy { get; private set; }

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    public partial string StatusMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AssignSelectedTechnicianCommand))]
    public partial TechnicianOption? SelectedTechnician { get; set; }

    public TicketDetailsViewModel(ITicketHandlingService service)
    {
        ticketHandlingService = service;
        Technicians = service.GetTechnicians();
    }

    public async Task LoadAsync(Guid ticketId)
    {
        if (IsBusy) throw new InvalidOperationException("Vänta tills den pågående åtgärden är klar.");
        IsBusy = true;
        StatusMessage = string.Empty;
        currentTicket = null;
        NewCommentText = string.Empty;
        UpdateDisplayedTicket(resetEditing: true);
        try
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("Välj ett ärende med ett giltigt id.", nameof(ticketId));
            currentTicket = await ticketHandlingService.GetTicketById(ticketId);
            UpdateDisplayedTicket(resetEditing: true);
        }
        catch (Exception)
        {
            StatusMessage = "Ärendet kunde inte läsas in. Kontrollera ärende-id och datafilen.";
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAddComment() => currentTicket != null && !IsBusy && !string.IsNullOrWhiteSpace(NewCommentText);

    [RelayCommand(CanExecute = nameof(CanAddComment))]
    private async Task AddCommentAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.AddCommentToTicket(CurrentTicketId, NewCommentText);
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

    private bool CanChangeTitle() => currentTicket != null && !IsBusy
        && !string.IsNullOrWhiteSpace(UpdateTitleText);

    [RelayCommand(CanExecute = nameof(CanChangeTitle))]
    private async Task UpdateTitleAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.ChangeTicketTitle(CurrentTicketId, UpdateTitleText);
            if (!result)
            {
                StatusMessage = "Rubriken kunde inte uppdateras.";
                return;
            }
            await RefreshAsync();
            UpdateTitleText = Title;
            StatusMessage = "Rubriken har uppdaterats.";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Rubriken kunde inte uppdateras. Kontrollera rubriken och uppgifterna i datafilen.";
        }
        catch (KeyNotFoundException)
        {
            StatusMessage = "Ärendet kunde inte hittas.";
        }
        catch (JsonException)
        {
            StatusMessage = "Datafilen innehåller felaktiga uppgifter. Rubriken kunde inte uppdateras.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine(ex);
            StatusMessage = "Rubriken kunde inte sparas. Kontrollera datafilen och åtkomsten.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            StatusMessage = "Ett oväntat fel uppstod när rubriken skulle uppdateras.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanChangeDescription() => currentTicket != null && !IsBusy
        && !string.IsNullOrWhiteSpace(UpdateDescriptionText);

    [RelayCommand(CanExecute = nameof(CanChangeDescription))]
    private async Task UpdateDescriptionAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.ChangeTicketDescription(CurrentTicketId, UpdateDescriptionText);
            if (!result)
            {
                StatusMessage = "Beskrivningen kunde inte uppdateras.";
                return;
            }
            await RefreshAsync();
            UpdateDescriptionText = Description;
            StatusMessage = "Beskrivningen har uppdaterats.";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Beskrivningen kunde inte uppdateras. Kontrollera beskrivningen och uppgifterna i datafilen.";
        }
        catch (KeyNotFoundException)
        {
            StatusMessage = "Ärendet kunde inte hittas.";
        }
        catch (JsonException)
        {
            StatusMessage = "Datafilen innehåller felaktiga uppgifter. Beskrivningen kunde inte uppdateras.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine(ex);
            StatusMessage = "Beskrivningen kunde inte sparas. Kontrollera datafilen och åtkomsten.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            StatusMessage = "Ett oväntat fel uppstod när beskrivningen skulle uppdateras.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanAssignTechnician() => currentTicket != null && !IsBusy && SelectedTechnician != null;

    [RelayCommand(CanExecute = nameof(CanAssignTechnician))]
    private async Task AssignSelectedTechnicianAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            if (SelectedTechnician == null) throw new ArgumentException("Välj en handläggare.");
            var result = await ticketHandlingService.AssignTechnician(CurrentTicketId, SelectedTechnician.TechnicianId);
            if (!result)
            {
                StatusMessage = "Handläggaren kunde inte tilldelas.";
                return;
            }
            await RefreshAsync();
            StatusMessage = "Handläggaren har tilldelats. Ärendet är nu Pågående.";
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

    private bool CanCloseTicket() => !IsBusy && currentTicket != null
        && currentTicket.AssignedTechnicianId.HasValue
        && currentTicket.Status != TicketStatusOption.Resolved;

    [RelayCommand(CanExecute = nameof(CanCloseTicket))]
    private async Task CloseTicketAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.CloseTicket(CurrentTicketId);
            if (!result)
            {
                StatusMessage = "Ärendet kunde inte stängas.";
                return;
            }
            await RefreshAsync();
            StatusMessage = "Ärendet har stängts och är nu Löst.";
        }
        catch (InvalidOperationException)
        {
            StatusMessage = "Tilldela en handläggare innan ärendet stängs.";
        }
        catch (Exception)
        {
            StatusMessage = "Ärendet kunde inte stängas. Kontrollera datafilen och försök igen.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanChangePriority() => currentTicket != null && !IsBusy && Enum.IsDefined(SelectedPriority);

    [RelayCommand(CanExecute = nameof(CanChangePriority))]
    private async Task ChangePriorityAsync()
    {
        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            var result = await ticketHandlingService.ChangeTicketPriority(CurrentTicketId, SelectedPriority);
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
        currentTicket = await ticketHandlingService.GetTicketById(CurrentTicketId);
        UpdateDisplayedTicket();
    }

    private void UpdateDisplayedTicket(bool resetEditing = false)
    {
        Comments.Clear();
        if (currentTicket != null)
            foreach (var comment in currentTicket.Comments) Comments.Add(comment);
        SelectedTechnician = currentTicket == null ? null
            : Technicians.FirstOrDefault(item => item.TechnicianId == currentTicket.AssignedTechnicianId);
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(Priority));
        OnPropertyChanged(nameof(TechnicianName));
        if (resetEditing)
        {
            UpdateTitleText = Title;
            UpdateDescriptionText = Description;
        }
        SelectedPriority = Priority;
        AddCommentCommand.NotifyCanExecuteChanged();
        AssignSelectedTechnicianCommand.NotifyCanExecuteChanged();
        ChangePriorityCommand.NotifyCanExecuteChanged();
        CloseTicketCommand.NotifyCanExecuteChanged();
        UpdateTitleCommand.NotifyCanExecuteChanged();
        UpdateDescriptionCommand.NotifyCanExecuteChanged();
    }

}
