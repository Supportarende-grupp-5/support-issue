
using SupportIssue.Application.Methods;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using System.Security.Cryptography.X509Certificates;

namespace SupportIssue.Presentation.ViewModels;

public class TicketDetailsViewModel
{
    private readonly SupportTicket CurrentSupportTicket;
    private readonly ITicketHandlingService ticketHandlingService;
    public string Title => CurrentSupportTicket.Title;
    public string Description => CurrentSupportTicket.Description;
    public TicketStatus Status => CurrentSupportTicket.Status;
    public TicketPriority Priority => CurrentSupportTicket.Priority;
    public string TechnicianName
    {
        get
        {
            if (CurrentSupportTicket.AssignedTechnician == null)
            {
                return "Ej tilldelad";
            }

            return CurrentSupportTicket.AssignedTechnician.TechnicianName
                ?? "Namn saknas";
        }
    }
    public List<TicketComment> Comments => CurrentSupportTicket.Comments;
    public string NewCommentText { get; set; } = string.Empty;
    public TicketDetailsViewModel(SupportTicket ticket, ITicketHandlingService service)
    {
        CurrentSupportTicket = ticket;
        ticketHandlingService = service;
    }
}
