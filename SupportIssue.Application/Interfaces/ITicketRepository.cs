using SupportIssue.Domain.Entities;

namespace SupportIssue.Application.Interfaces;

public interface ITicketRepository
{
    Task AddAsync(SupportTicket ticket);

    Task<IReadOnlyList<SupportTicket>> GetAllAsync();
}
