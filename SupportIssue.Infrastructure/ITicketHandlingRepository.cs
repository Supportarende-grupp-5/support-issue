using SupportIssue.Domain;

namespace SupportIssue.Infrastructure;

public interface ITicketHandlingRepository
{
    Task<Ticket> GetTicketByIdAsync(Guid ticketId);
    Task<IEnumerable<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId);
}
