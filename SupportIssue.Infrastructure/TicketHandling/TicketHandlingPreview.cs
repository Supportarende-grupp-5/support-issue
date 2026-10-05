using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Infrastructure.TicketHandling;

// Tillfällig förhandsvisning tills gruppens ärendelista kopplas in.
// Exempeldata hålls i minnet och påverkar inte de vanliga JSON-filerna.
public static class TicketHandlingPreview
{
    public static async Task<(ITicketHandlingService Service, TicketDetails Ticket)> CreateAsync()
    {
        var ticket = new SupportTicket("Skrivaren fungerar inte",
            "Kontorets skrivare tar emot utskrifter men skriver inte ut några sidor.",
            Guid.NewGuid(), TicketPriority.Normal);
        ticket.AssignTechnician(Technician.CreateTechnicianList().First());
        ticket.UpdateStatus(TicketStatus.InProgress);
        ticket.AddComment("Kontrollerat att skrivaren är ansluten till nätverket.");
        ticket.AddComment("Startat om skrivaren och skickat en provutskrift.");
        ITicketHandlingService service = new TicketHandlingService(new PreviewRepository(ticket));
        return (service, await service.GetTicketById(ticket.Id));
    }

    private class PreviewRepository(SupportTicket ticket) : ITicketHandlingRepository
    {
        public Task<SupportTicket> GetTicketByIdAsync(Guid ticketId)
        {
            if (ticketId != ticket.Id) throw new KeyNotFoundException("Ärendet hittades inte.");
            return Task.FromResult(SupportTicket.Restore(ticket.Id, ticket.CustomerId,
                ticket.Title, ticket.Description, ticket.Priority, ticket.Status,
                ticket.CreatedAt, ticket.AssignedTechnician, ticket.Comments));
        }
        public async Task<List<SupportTicket>> GetAllTicketsAsync() => [await GetTicketByIdAsync(ticket.Id)];
        public Task<bool> SaveTicketAsync(SupportTicket updatedTicket)
        {
            if (updatedTicket.Id != ticket.Id) throw new KeyNotFoundException("Ärendet hittades inte.");
            ticket = updatedTicket;
            return Task.FromResult(true);
        }
    }
}
