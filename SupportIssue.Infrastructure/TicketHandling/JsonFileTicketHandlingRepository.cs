using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using System.Text.Json;

namespace SupportIssue.Infrastructure.TicketHandling;

public class JsonFileTicketHandlingRepository : ITicketHandlingRepository
{
    private readonly string _ticketfilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "SupportIssue", "tickets.json");

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task<SupportTicket> GetTicketByIdAsync(Guid ticketId)
    {
        var models = await ReadStorageModelsAsync();
        var model = models.FirstOrDefault(ticket => ticket.Id == ticketId);
        if (model == null)
            throw new KeyNotFoundException($"Ticket with ID {ticketId} was not found.");

        return RestoreTicket(model);
    }

    public async Task<List<SupportTicket>> GetAllTicketsAsync()
    {
        var models = await ReadStorageModelsAsync();
        return models.Select(RestoreTicket).ToList();
    }

    private async Task<List<TicketStorageModel>> ReadStorageModelsAsync()
    {
        if (!File.Exists(_ticketfilePath))
            throw new FileNotFoundException("The ticket file was not found.", _ticketfilePath);

        var json = await File.ReadAllTextAsync(_ticketfilePath);
        return JsonSerializer.Deserialize<List<TicketStorageModel>>(json, _options)
            ?? throw new JsonException("The ticket file must contain a list of tickets.");
    }

    private static SupportTicket RestoreTicket(TicketStorageModel model)
    {
        if (model == null) throw new JsonException("The ticket list contains an empty entry.");
        if (model.Comments == null) throw new JsonException("The ticket's comment list is missing.");

        Technician? technician = null;
        if (model.AssignedTechnicianId.HasValue)
        {
            technician = Technician.CreateTechnicianList()
                .FirstOrDefault(item => item.TechnicianId == model.AssignedTechnicianId.Value);
            if (technician == null)
                throw new JsonException($"Technician with ID {model.AssignedTechnicianId} was not found.");
        }

        var comments = new List<TicketComment>();
        foreach (var comment in model.Comments)
        {
            if (comment == null) throw new JsonException("The comment list contains an empty entry.");
            comments.Add(TicketComment.Restore(comment.Id, comment.TicketId,
                comment.Comment, comment.CreatedAt));
        }

        return SupportTicket.Restore(model.Id, model.CustomerId, model.Title,
            model.Description, model.Priority, model.Status, model.CreatedAt,
            technician, comments);
    }

    public Task<bool> SaveTicketAsync(SupportTicket ticket)
    {
        // Placeholder inför merge: uppdatera via id och skriv listan till fil.
        return Task.FromResult(true);
    }
}
