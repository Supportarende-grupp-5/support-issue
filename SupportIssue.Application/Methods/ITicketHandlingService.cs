
using SupportIssue.Domain;
using static SupportIssue.Domain.Ticket;

namespace SupportIssue.Application.Methods;

public interface ITicketHandlingService
{
    public Task<Ticket> GetTicketById(Guid ticketId);
    public Task<bool> AssignTechnician(Ticket ticket, int technicianId);
    public Task<Ticket> ChangeTicketStatus(Ticket ticket, TicketStatus newStatus);
    public Task<bool> AddCommentToTicket(Ticket ticket, string comment);
    public Task<Ticket> ChangeTicketPriority(Ticket ticket, TicketPriority newPriority);
    public Task<List<TicketComment>> GetCommentsByTicket(Ticket ticket);

}
