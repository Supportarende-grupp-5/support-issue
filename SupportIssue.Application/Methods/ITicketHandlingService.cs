using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Methods;

public interface ITicketHandlingService
{
    public Task<SupportTicket> GetTicketById(Guid ticketId);
    public Task<bool> AssignTechnician(SupportTicket ticket, int technicianId);
    public Task<SupportTicket> ChangeTicketStatus(SupportTicket ticket, TicketStatus newStatus);
    public Task<bool> AddCommentToTicket(SupportTicket ticket, string comment);
    public Task<SupportTicket> ChangeTicketPriority(SupportTicket ticket, TicketPriority newPriority);
    public Task<List<TicketComment>> GetCommentsByTicket(SupportTicket ticket);

}
