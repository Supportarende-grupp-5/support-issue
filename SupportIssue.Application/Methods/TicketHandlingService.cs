using SupportIssue.Domain;

namespace SupportIssue.Application.Methods;

public class TicketHandlingService : ITicketHandlingService
{
    public Task<Ticket> AddCommentToTicket(Guid ticketId, string comment)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> AssignTechnician(Guid ticketId, Guid technicianId)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> ChangePriority(Guid ticketId, string newPriority)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> ChangeTicketStatus(Guid ticketId, string newStatus)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> EditTicket(Guid technicianId, string ticketTitle, string ticketDescription)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TicketComment>> GetCommentsByTicketId(Guid ticketId)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket> GetTicketById(Guid ticketId)
    {
        try
        {
            await GetTicketById(ticketId);
            return Ticket;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while retrieving the ticket.", ex);
        }
    }
}
