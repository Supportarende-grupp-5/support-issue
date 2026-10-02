using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.TicketHandling;

public interface ITicketHandlingRepository
{
    Task<SupportTicket> GetTicketByIdAsync(Guid ticketId);
    Task<List<SupportTicket>> GetAllTicketsAsync();
    Task<bool> SaveTicketAsync(SupportTicket ticket);
}
