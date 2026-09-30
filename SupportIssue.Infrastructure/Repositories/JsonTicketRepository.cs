using System.Text.Json;
using System.Text.Json.Serialization;
using SupportIssue.Application.Interfaces;
using SupportIssue.Domain.Entities;
using SupportIssue.Infrastructure.Models;

namespace SupportIssue.Infrastructure.Repositories;

public class JsonTicketRepository : ITicketRepository
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

    public async Task AddAsync(SupportTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        await _fileLock.WaitAsync();

        try
        {
            var tickets = await ReadTicketsAsync();

            foreach (var existingTicket in tickets)
            {
                if (existingTicket.Id == ticket.Id)
                {
                    throw new InvalidOperationException(
                        "A ticket with this ID already exists.");
                }
            }

            tickets.Add(ticket);

            var fileModels = new List<TicketFileModel>();

            foreach (var item in tickets)
            {
                fileModels.Add(new TicketFileModel
                {
                    Id = item.Id,
                    CustomerId = item.CustomerId,
                    Title = item.Title,
                    Description = item.Description,
                    TicketPriority = item.Priority,
                    TicketStatus = item.Status,
                    CreatedAt = item.CreatedAt
                });
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

                var ticket = SupportTicket.Restore(
                    model.Id,
                    model.Title,
                    model.Description,
                    model.CustomerId,
                    model.TicketPriority,
                    model.TicketStatus,
                    model.CreatedAt);

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
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
