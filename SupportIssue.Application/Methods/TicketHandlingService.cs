namespace SupportIssue.Application.Methods;

public class TicketHandlingService : ITicketHandlingService
{
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
