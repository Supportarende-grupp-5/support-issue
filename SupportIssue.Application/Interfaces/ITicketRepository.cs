using SupportIssue.Domain.Entities;

namespace SupportIssue.Application.Interfaces;

public interface ITicketRepository
{
    void Add(SupportTicket ticket);
}
