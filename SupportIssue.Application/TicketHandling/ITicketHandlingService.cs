namespace SupportIssue.Application.TicketHandling;

public interface ITicketHandlingService
{
    Task<TicketDetails> GetTicketById(Guid ticketId);
    IReadOnlyList<TechnicianOption> GetTechnicians();
    Task<bool> AssignTechnician(Guid ticketId, int technicianId);
    Task<bool> CloseTicket(Guid ticketId);
    Task<bool> ChangeTicketStatus(Guid ticketId, TicketStatusOption newStatus);
    Task<bool> AddCommentToTicket(Guid ticketId, string comment);
    Task<bool> ChangeTicketPriority(Guid ticketId, TicketPriorityOption newPriority);
}
