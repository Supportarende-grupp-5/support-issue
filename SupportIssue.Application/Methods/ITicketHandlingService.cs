
using SupportIssue.Domain;
using static SupportIssue.Domain.Ticket;

namespace SupportIssue.Application.Methods;

public interface ITicketHandlingService
{
    public Task<Ticket> GetTicketById(Guid ticketId);
    public Task<Ticket> EditTicket(Guid ticketId, string ticketTitle, string ticketDescription);
    public Task<Ticket> AssignTechnician(Guid ticketId, Guid technicianId);
    public Task<Ticket> ChangeTicketStatus(Guid ticketId, TicketStatus newStatus);
    public Task<Ticket> AddCommentToTicket(Guid ticketId, string comment);
    public Task<Ticket> ChangePriority(Guid ticketId, TicketPriority newPriority);
    public Task<IEnumerable<TicketComment>> GetCommentsByTicketId(Guid ticketId);

}
