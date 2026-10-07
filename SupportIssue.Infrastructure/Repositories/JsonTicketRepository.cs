using System.Text.Json;
using System.Text.Json.Serialization;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Infrastructure.Models;
using SupportIssue.Infrastructure.TicketHandling;

namespace SupportIssue.Infrastructure.Repositories;

public class JsonTicketRepository : ITicketRepository, ITicketHandlingRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public JsonTicketRepository(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        _filePath = Path.GetFullPath(filePath);
    }

    public async Task<IReadOnlyList<SupportTicket>> GetAllAsync()
    {
        await _fileLock.WaitAsync();

        try
        {
            return await ReadTicketsAsync();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public Task AddAsync(SupportTicket ticket)
    {
        return SaveAsync(ticket, allowUpdate: false);
    }

    public async Task<bool> SaveTicketAsync(SupportTicket ticket)
    {
        await SaveAsync(ticket, allowUpdate: true);
        return true;
    }

    public async Task<List<SupportTicket>> GetAllTicketsAsync()
    {
        var tickets = await GetAllAsync();
        return tickets.ToList();
    }

    public async Task<SupportTicket> GetTicketByIdAsync(Guid ticketId)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException(
                "Ticket ID is required.",
                nameof(ticketId));
        }

        var tickets = await GetAllAsync();

        return tickets.FirstOrDefault(ticket => ticket.Id == ticketId)
            ?? throw new KeyNotFoundException(
                $"Ticket with ID {ticketId} was not found.");
    }

    private async Task SaveAsync(SupportTicket ticket, bool allowUpdate)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        await _fileLock.WaitAsync();

        try
        {
            var tickets = await ReadTicketsAsync();

            int index = tickets.FindIndex(
                existingTicket => existingTicket.Id == ticket.Id);

            if (index >= 0)
            {
                if (!allowUpdate)
                {
                    throw new InvalidOperationException(
                        "A ticket with this ID already exists.");
                }

                tickets[index] = ticket;
            }
            else
            {
                tickets.Add(ticket);
            }

            var fileModels = new List<TicketFileModel>();

            foreach (var item in tickets)
            {
                var model = new TicketFileModel
                {
                    Id = item.Id,
                    CustomerId = item.CustomerId,
                    Title = item.Title,
                    Description = item.Description,
                    TicketPriority = item.Priority,
                    TicketStatus = item.Status,
                    CreatedAt = item.CreatedAt,
                    AssignedTechnicianId = item.AssignedTechnicianId,

                    Comments = item.Comments.Select(comment =>
                        new TicketCommentStorageModel
                        {
                            Id = comment.Id,
                            TicketId = comment.TicketId,
                            Comment = comment.Comment,
                            CreatedAt = comment.CreatedAt
                        }).ToList()
                };

                // Validera uppgifterna innan filen skrivs.
                RestoreTicket(model);

                fileModels.Add(model);
            }

            string json = JsonSerializer.Serialize(
                fileModels,
                _jsonOptions);

            await WriteFileAsync(json);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private async Task<List<SupportTicket>> ReadTicketsAsync()
    {
        string json;

        try
        {
            json = await File.ReadAllTextAsync(_filePath);
        }
        catch (FileNotFoundException)
        {
            return new List<SupportTicket>();
        }
        catch (DirectoryNotFoundException)
        {
            return new List<SupportTicket>();
        }

        try
        {
            var fileModels =
                JsonSerializer.Deserialize<List<TicketFileModel>>(
                    json,
                    _jsonOptions);

            if (fileModels is null)
            {
                throw new InvalidDataException(
                    "The ticket file must contain a list.");
            }

            var tickets = new List<SupportTicket>();
            var ids = new HashSet<Guid>();

            foreach (var model in fileModels)
            {
                if (model is null)
                {
                    throw new InvalidDataException(
                        "The ticket file contains an empty entry.");
                }

                var ticket = RestoreTicket(model);

                if (!ids.Add(ticket.Id))
                {
                    throw new InvalidDataException(
                        "The ticket file contains duplicate IDs.");
                }

                tickets.Add(ticket);
            }

            return tickets;
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                "The ticket file has an invalid or unsupported format.",
                ex);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidDataException(
                "The ticket file contains invalid or unsupported ticket data.",
                ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidDataException(
                "The ticket file contains an invalid ticket state.",
                ex);
        }
    }

    private static SupportTicket RestoreTicket(TicketFileModel model)
    {
        if (model.Comments is null)
        {
            throw new InvalidDataException(
                "The ticket's comment list cannot be null.");
        }

        Technician? technician = null;

        if (model.AssignedTechnicianId.HasValue)
        {
            technician = Technician.CreateTechnicianList()
                .FirstOrDefault(item =>
                    item.TechnicianId == model.AssignedTechnicianId.Value);

            if (technician is null)
            {
                throw new InvalidDataException(
                    "The ticket refers to an unknown technician.");
            }
        }

        var comments = new List<TicketComment>();
        var commentIds = new HashSet<Guid>();

        foreach (var comment in model.Comments)
        {
            if (comment is null)
            {
                throw new InvalidDataException(
                    "The comment list contains an empty entry.");
            }

            if (!commentIds.Add(comment.Id))
            {
                throw new InvalidDataException(
                    "The ticket contains duplicate comment IDs.");
            }

            comments.Add(TicketComment.Restore(
                comment.Id,
                comment.TicketId,
                comment.Comment,
                comment.CreatedAt));
        }

        return SupportTicket.Restore(
            model.Id,
            model.CustomerId,
            model.Title,
            model.Description,
            model.TicketPriority,
            model.TicketStatus,
            model.CreatedAt,
            technician,
            comments);
    }

    private async Task WriteFileAsync(string json)
    {
        string directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);

        string temporaryPath =
            _filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await File.WriteAllTextAsync(temporaryPath, json);

            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            try
            {
                File.Delete(temporaryPath);
            }
            catch (IOException)
            {
                // Ett städfel ska inte dölja det ursprungliga felet.
            }
            catch (UnauthorizedAccessException)
            {
                // Ett städfel ska inte dölja det ursprungliga felet.
            }
        }
    }
}