
namespace SupportIssue.Application.Methods;

public interface ITicketHandlingService
{
    public Task<Ticket> GetTicketById(Guid ticketId);
    public Task<Ticket> EditTicket(Guid technicianId, string ticketTitle, string ticketDescription);
    public Task<Ticket> AssignTechnician(Guid ticketId, Guid technicianId);
    public Task<Ticket> ChangeTicketStatus(Guid ticketId, string newStatus);
    public Task<Ticket> AddCommentToTicket(Guid ticketId, string comment);
    public Task<Ticket> ChangePriority(Guid ticketId, string newPriority);
    public Task<IEnumerable<Comment>> GetCommentsByTicketId(Guid ticketId);

}
