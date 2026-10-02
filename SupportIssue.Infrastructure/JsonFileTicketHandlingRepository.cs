using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain.Entities;
using System.Text.Json;

namespace SupportIssue.Infrastructure;

public class JsonFileTicketHandlingRepository : ITicketHandlingRepository
{
    private readonly string _ticketfilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "SupportIssue",
        "tickets.json"
        );
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true

    };
    public async Task<bool> AddCommentToTicketAsync(SupportTicket ticket, string comment)
    {
        return true;

    }
    public async Task<SupportTicket> GetTicketByIdAsync(Guid ticketId)
    {
        {
            if (!File.Exists(_ticketfilePath))
            {
                throw new FileNotFoundException("The ticket file was not found.", _ticketfilePath);
            }
            var json = await File.ReadAllTextAsync(_ticketfilePath);
            var tickets = JsonSerializer.Deserialize<List<SupportTicket>>(json);
            var ticket = tickets?.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {ticketId} was not found.");
            }
            return ticket;
        }
    }
    public async Task<List<SupportTicket>> GetAllTicketsAsync()
        {
        if (!File.Exists(_ticketfilePath))
        {
            throw new FileNotFoundException("The ticket file was not found.", _ticketfilePath);
        }
        var json = await File.ReadAllTextAsync(_ticketfilePath);
        var allTickets = JsonSerializer.Deserialize<List<SupportTicket>>(json);
        if (allTickets == null)
        {
            throw new KeyNotFoundException($"No Tickets found");
        }
       

        return allTickets;


    }
    public async Task<bool> SaveAllAsync(List<SupportTicket> tickets)
    {
        return true;
    }
    public async Task<bool> SaveTicketAsync(SupportTicket ticket)
    {
        var list = await GetAllTicketsAsync();
        list.Add(ticket);
        return true;
    }

 
}