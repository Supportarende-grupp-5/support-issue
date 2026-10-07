using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using System.Text.Json;

namespace SupportIssue.Infrastructure.TicketHandling;

public class JsonFileTicketHandlingRepository : ITicketHandlingRepository
{
    private readonly string _ticketfilePath;

    public JsonFileTicketHandlingRepository() : this(Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "SupportIssue", "tickets.json"))
    {
    }

    public JsonFileTicketHandlingRepository(string ticketFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticketFilePath);
        _ticketfilePath = Path.GetFullPath(ticketFilePath);
    }

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
            return [];

        var json = await File.ReadAllTextAsync(_ticketfilePath);
        var models = JsonSerializer.Deserialize<List<TicketStorageModel>>(json, _options)
            ?? throw new JsonException("The ticket file must contain a list of tickets.");

        var ids = new HashSet<Guid>();
        foreach (var model in models)
        {
            RestoreTicket(model);
            if (!ids.Add(model.Id))
                throw new JsonException("The ticket file contains duplicate ticket ids.");
        }
        return models;
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

    public async Task<bool> SaveTicketAsync(SupportTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        var models = await ReadStorageModelsAsync();
        var model = new TicketStorageModel
        {
            Id = ticket.Id,
            CustomerId = ticket.CustomerId,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            AssignedTechnicianId = ticket.AssignedTechnicianId,
            Comments = ticket.Comments.Select(comment => new TicketCommentStorageModel
            {
                Id = comment.Id,
                TicketId = comment.TicketId,
                Comment = comment.Comment,
                CreatedAt = comment.CreatedAt
            }).ToList()
        };
        RestoreTicket(model);

        var index = models.FindIndex(item => item.Id == ticket.Id);
        if (index >= 0)
            models[index] = model;
        else
            models.Add(model);

        var json = JsonSerializer.Serialize(models, _options);
        Directory.CreateDirectory(Path.GetDirectoryName(_ticketfilePath)!);
        var temporaryPath = _ticketfilePath + "." + Guid.NewGuid() + ".tmp";
        try
        {
            await File.WriteAllTextAsync(temporaryPath, json);
            File.Move(temporaryPath, _ticketfilePath, overwrite: true);
        }
        finally
        {
            try { File.Delete(temporaryPath); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
        return true;
    }
}
