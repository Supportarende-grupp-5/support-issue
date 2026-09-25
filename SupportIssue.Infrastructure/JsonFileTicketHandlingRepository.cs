using SupportIssue.Domain;
using System.Text.Json;

namespace SupportIssue.Infrastructure;

public class JsonFileTicketHandlingRepository : ITicketHandlingRepository
{
    private readonly string _commentfilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "SupportIssue",
        "comments.json"
        );
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
    public async Task<TicketComment> AddCommentToTicketAsync(Guid ticketId, string comment, DateTime createdAt)
    {
        if (!File.Exists(_commentfilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_commentfilePath)!);
        }
        var ticketComment = new TicketComment
        {
            TicketId = ticketId,
            Comment = comment,
            CreatedAt = createdAt
        };
        return ticketComment;

    }
    public async Task<Ticket> GetTicketByIdAsync(Guid ticketId)
    {
        {
            if (!File.Exists(_ticketfilePath))
            {
                throw new FileNotFoundException("The ticket file was not found.", _ticketfilePath);
            }
            var json = await File.ReadAllTextAsync(_ticketfilePath);
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
        if (!File.Exists(_commentfilePath))
        {
            throw new FileNotFoundException("The ticket comment file was not found.", _commentfilePath);
        }
        var json = await File.ReadAllTextAsync(_commentfilePath);
        var Allcomments = JsonSerializer.Deserialize<List<TicketComment>>(json);
        if (Allcomments == null)
        {
            throw new KeyNotFoundException($"No comments found");
        }
        var comments = Allcomments.Where(c => c.TicketId == ticketId).ToList();

        return comments;
    }



    public async Task SaveAllAsync(IEnumerable<TicketComment> comments)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_commentfilePath)!);
        string json = JsonSerializer.Serialize(comments, _options);
        var tempPath = _commentfilePath + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, _commentfilePath, true);
    }
}
