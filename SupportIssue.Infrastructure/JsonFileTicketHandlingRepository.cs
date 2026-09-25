using SupportIssue.Domain;
using System.Text.Json;

namespace SupportIssue.Infrastructure;

public class JsonFileTicketHandlingRepository : ITicketHandlingRepository
{
    public async Task<Ticket> GetTicketByIdAsync(Guid ticketId)
    {
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("The ticket file was not found.", _filePath);
            }
            var json = await File.ReadAllTextAsync(_filePath);
            var tickets = JsonSerializer.Deserialize<List<Ticket>>(json);
            var ticket = tickets?.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {ticketId} was not found.");
            }
            return ticket;
        }
    }
    public async Task<IEnumerable<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("The ticket comment file was not found.", _filePath);
        }
        var json = await File.ReadAllTextAsync(_filePath);
        var Allcomments = JsonSerializer.Deserialize<List<TicketComment>>(json);
        if (Allcomments == null)
        {
            throw new KeyNotFoundException($"No comments found");
        }
        var comments = Allcomments.Where(c => c.TicketId == ticketId).ToList();

        return comments;
    }
}
