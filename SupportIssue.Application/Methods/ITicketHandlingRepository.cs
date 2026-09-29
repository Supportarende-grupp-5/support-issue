using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Methods;

public interface ITicketHandlingRepository
{
    Task<SupportTicket> GetTicketByIdAsync(Guid ticketId);
    Task<List<TicketComment>> GetCommentsByTicketAsync(SupportTicket ticket);
    Task<bool> AddCommentToTicketAsync(SupportTicket ticket, string comment);
    Task<List<TicketComment>> GetAllCommentsAsync();
    Task<bool> AssignTechnicianAsync(SupportTicket ticket, int technicianId);
    Task<bool> ChangeTicketStatusAsync(SupportTicket ticket, TicketStatus newstatus);
    Task<bool> ChangeTicketPriorityAsync(SupportTicket ticket, TicketPriority newPriority);
}
