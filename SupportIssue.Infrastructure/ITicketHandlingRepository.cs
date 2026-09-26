using SupportIssue.Domain;
using static SupportIssue.Domain.Ticket;

namespace SupportIssue.Infrastructure;

public interface ITicketHandlingRepository
{
    Task<Ticket> GetTicketByIdAsync(Guid ticketId);
    Task<List<TicketComment>> GetCommentsByTicketAsync(Ticket ticket);
    Task<bool> AddCommentToTicketAsync(Ticket ticket, string comment);
    Task<List<TicketComment>> GetAllCommentsAsync();
    Task<bool> AssignTechnicianAsync(Ticket ticket, int technicianId);
    Task<bool> ChangeTicketStatusAsync(Ticket ticket, TicketStatus newstatus);
    Task<bool> ChangeTicketPriorityAsync(Ticket ticket, TicketPriority newPriority);
}
