using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using SupportIssue.Infrastructure.Repositories;


namespace SupportIssue.Tests;

public class JsonTicketRepositoryTests
{
    [Fact]
    public async Task AddAsync_SavesTicket_ThatCanBeReadBack()
    {
        // Arrange
        var testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"SupportIssueTests_{Guid.NewGuid():N}");

        Directory.CreateDirectory(testDirectory);

        var filePath = Path.Combine(testDirectory, "ticket.json");

        try
        {
            var repository = new JsonTicketRepository(filePath);

            var ticket = new SupportTicket(
                "Cannot log in",
                "An error appears when logging in.",
                Guid.NewGuid(),
                TicketPriority.Normal);

            // Act
            await repository.AddAsync(ticket);

            var readRepository = new JsonTicketRepository(filePath);
            var savedTickets = await readRepository.GetAllAsync();

            // Assert
            var savedTicket = Assert.Single(savedTickets);

            Assert.Equal(ticket.Id, savedTicket.Id);
            Assert.Equal(ticket.CustomerId, savedTicket.CustomerId);
            Assert.Equal(ticket.Title, savedTicket.Title);
            Assert.Equal(ticket.Description, savedTicket.Description);
            Assert.Equal(ticket.Priority, savedTicket.Priority);
            Assert.Equal(ticket.Status, savedTicket.Status);
            Assert.Equal(ticket.CreatedAt, savedTicket.CreatedAt);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task AddAsync_PreservesExistingTickets()
    {
        var testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"SupportIssueTests_{Guid.NewGuid():N}");

        Directory.CreateDirectory(testDirectory);

        var filePath = Path.Combine(testDirectory, "tickets.json");

        try
        {
            // Förbered två olika ärenden.
            var firstTicket = new SupportTicket(
                "Cannot log in",
                "An error appears when logging in.",
                Guid.NewGuid(),
                TicketPriority.Normal);

            var secondTicket = new SupportTicket(
                "Printer not working",
                "The printer does not print.",
                Guid.NewGuid(),
                TicketPriority.High);

            // Spara det första ärendet.
            var repository = new JsonTicketRepository(filePath);
            await repository.AddAsync(firstTicket);

            // Spara det andra genom en ny repository instans.
            var secondRepository = new JsonTicketRepository(filePath);
            await secondRepository.AddAsync(secondTicket);

            // Läs tillbaka ärendena från filen.
            var readRepository = new JsonTicketRepository(filePath);
            var savedTickets = await readRepository.GetAllAsync();

            // Kontrollera att båda finns kvar.
            Assert.Equal(2, savedTickets.Count);

            Assert.Contains(
                savedTickets,
                ticket => ticket.Id == firstTicket.Id);

            Assert.Contains(
                savedTickets,
                ticket => ticket.Id == secondTicket.Id);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task AddAsync_InvalidJson_DoesNotOverwriteFile()
    {
        var testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"SupportIssueTests_{Guid.NewGuid():N}");

        Directory.CreateDirectory(testDirectory);

        var filePath = Path.Combine(testDirectory, "tickets.json");

        try
        {
            // Arrange: skapa en fil med trasig JSON.
            var invalidJson = "{ invalid json";

            await File.WriteAllTextAsync(filePath, invalidJson);

            var repository = new JsonTicketRepository(filePath);

            var ticket = new SupportTicket(
                "Cannot log in",
                "An error appears when logging in.",
                Guid.NewGuid(),
                TicketPriority.Normal);

            // Act och Assert: sparandet ska avbrytas med ett fel.
            await Assert.ThrowsAsync<InvalidDataException>(
                () => repository.GetAllAsync());

            // Kontrollera att filens innehåll är oförändrat.
            var contentAfter = await File.ReadAllTextAsync(filePath);

            Assert.Equal(invalidJson, contentAfter);
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }
}
