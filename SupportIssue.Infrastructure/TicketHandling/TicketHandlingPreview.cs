using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Infrastructure.TicketHandling;

// Tillfällig förhandsvisning tills gruppens ärendelista kopplas in.
// Exempeldata sparas i en separat testfil tills den riktiga ärendelistan kopplas in.
public static class TicketHandlingPreview
{
    public static Task<(ITicketHandlingService Service, TicketDetails Ticket)> CreateAsync() =>
        CreateAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "SupportIssue", "preview-tickets.json"));

    public static async Task<(ITicketHandlingService Service, TicketDetails Ticket)> CreateAsync(string ticketFilePath)
    {
        var repository = new JsonFileTicketHandlingRepository(ticketFilePath);
        var tickets = await repository.GetAllTicketsAsync();
        var ticket = tickets.FirstOrDefault();

        // Skapa exemplet endast om testfilen saknas eller innehåller en tom lista.
        // Läsfel får gå vidare till anroparen utan att filen skrivs över.
        if (ticket == null)
        {
            ticket = CreateExampleTicket();
            if (!await repository.SaveTicketAsync(ticket))
                throw new IOException("Exempelärendet kunde inte sparas.");
        }

        ITicketHandlingService service = new TicketHandlingService(repository);
        return (service, await service.GetTicketById(ticket.Id));
    }

    private static SupportTicket CreateExampleTicket()
    {
        var ticket = new SupportTicket("Skrivaren fungerar inte",
            "Kontorets skrivare tar emot utskrifter men skriver inte ut några sidor.",
            Guid.NewGuid(), TicketPriority.Normal);
        ticket.AssignTechnician(Technician.CreateTechnicianList().First());
        ticket.UpdateStatus(TicketStatus.InProgress);
        ticket.AddComment("Kontrollerat att skrivaren är ansluten till nätverket.");
        ticket.AddComment("Startat om skrivaren och skickat en provutskrift.");
        return ticket;
    }
}
