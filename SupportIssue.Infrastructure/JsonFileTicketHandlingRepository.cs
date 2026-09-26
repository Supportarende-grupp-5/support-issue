using SupportIssue.Domain;
using System.Net.Sockets;
using System.Text.Json;
using static SupportIssue.Domain.Ticket;

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
    public async Task<bool> AddCommentToTicketAsync(Ticket ticket, string comment)
    {
        if (!File.Exists(_commentfilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_commentfilePath)!);
        }

        var ticketComment = new TicketComment(ticket.Id, comment);

        var allComments = await GetAllCommentsAsync();
        allComments.Add(ticketComment);
        bool saved = await SaveAllAsync(allComments);
        if (saved)
            return true;
        else
            return false;


    }
    //tillfällig lösning tills vi mergat
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
    public async Task<List<TicketComment>> GetCommentsByTicketAsync(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        if (!File.Exists(_commentfilePath))
        {
            throw new FileNotFoundException("The ticket comment file was not found.", _commentfilePath);
        }
        var json = await File.ReadAllTextAsync(_commentfilePath);
        var allComments = JsonSerializer.Deserialize<List<TicketComment>>(json);
        if (allComments == null)
        {
            throw new KeyNotFoundException($"No comments found");
        }
        var comments = allComments.Where(c => c.TicketId == ticket.Id).ToList();

        return comments;
    }



    public async Task<bool> SaveAllAsync(List<TicketComment> comments)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_commentfilePath)!);
        string json = JsonSerializer.Serialize(comments, _options);
        var tempPath = _commentfilePath + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, _commentfilePath, true);
        return true;
    }

    public async Task<List<TicketComment>> GetAllCommentsAsync()
    {
        if (!File.Exists(_commentfilePath))
        {
            throw new FileNotFoundException("The ticket comment file was not found.", _commentfilePath);
        }
        var json = await File.ReadAllTextAsync(_commentfilePath);
        var allComments = JsonSerializer.Deserialize<List<TicketComment>>(json);
        if (allComments == null)
        {
            throw new KeyNotFoundException($"No comments found");
        }
        return allComments;
    }
    public async Task<bool> AssignTechnicianAsync(Ticket ticket, int technicianId)
    {
        ticket.TechnicianId = technicianId;
        bool saved = await SaveAllTicketsAsync(ticket);
        return saved;
    }
    public async Task<bool> ChangeTicketStatusAsync(Ticket ticket, TicketStatus newStatus)
    {
        ticket.Status = newStatus;
        bool saved = await SaveAllTicketsAsync(ticket);
        return saved;
    }
    public async Task<bool> ChangeTicketPriorityAsync(Ticket ticket, TicketPriority newPriority)
    {
        ticket.Priority = newPriority;
        bool saved = await SaveAllTicketsAsync(ticket);
        return saved;
    }

    public async Task<bool> SaveAllTicketsAsync(Ticket ticket)
    {
        throw new NotImplementedException();
    }
}
