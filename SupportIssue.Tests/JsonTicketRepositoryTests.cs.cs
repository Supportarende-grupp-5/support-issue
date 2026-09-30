using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using SupportIssue.Infrastructure.Repositories;


namespace SupportIssue.Tests;

public class JsonTicketRepositoryTests
{
    [Fact]
    public async Task AddAsync_SavesTicket_ThatCanBeReadBack()
    {
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

            await repository.AddAsync(ticket);

            var readRepository = new JsonTicketRepository(filePath);
            var savedTickets = await readRepository.GetAllAsync();

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
}
