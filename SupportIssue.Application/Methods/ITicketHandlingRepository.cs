using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Methods;

public interface ITicketHandlingRepository
{
    Task<SupportTicket> GetTicketByIdAsync(Guid ticketId);
    Task<bool> AddCommentToTicketAsync(SupportTicket ticket, string comment);
    Task<List<SupportTicket>> GetAllTicketsAsync();
    Task<bool> SaveTicketAsync(SupportTicket ticket);
}
