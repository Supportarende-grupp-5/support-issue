using SupportIssue.Application.Interfaces;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using SupportIssue.Infrastructure.Repositories;
using Xunit;

namespace SupportIssue.Tests;

public class SharedTicketRepositoryTests
{
    [Fact]
    public async Task AddAsync_PreservesExistingTicketDetails()
    {
        // Arrange: skapa en separat mapp för testets fil.
        string testDirectory = Path.Combine(
            Path.GetTempPath(),
            "SupportIssueTests",
            Guid.NewGuid().ToString("N"));

        string filePath = Path.Combine(
            testDirectory,
            "tickets.json");

        try
        {
            var repository = new JsonTicketRepository(filePath);

            ITicketRepository registrationRepository = repository;
            ITicketHandlingRepository handlingRepository = repository;

            var firstTicket = new SupportTicket(
                "Printer not working",
                "The printer does not respond.",
                Guid.NewGuid(),
                TicketPriority.Normal);

            await registrationRepository.AddAsync(firstTicket);

            // Ärendehanteringen hämtar och uppdaterar ärendet.
            var ticketToUpdate =
                await handlingRepository.GetTicketByIdAsync(firstTicket.Id);

            var technician = Technician.CreateTechnicianList().First();

            ticketToUpdate.AssignTechnician(technician);
            ticketToUpdate.AddComment("Checked the printer connection.");
            ticketToUpdate.UpdatePriority(TicketPriority.High);

            var expectedComment = ticketToUpdate.Comments.Single();

            bool saved =
                await handlingRepository.SaveTicketAsync(ticketToUpdate);

            Assert.True(saved);

            // Act: registrera ytterligare ett ärende.
            var secondTicket = new SupportTicket(
                "Cannot log in",
                "The user cannot access their account.",
                Guid.NewGuid(),
                TicketPriority.Low);

            await registrationRepository.AddAsync(secondTicket);

            // Läs från filen med ett nytt repository.
            var reopenedRepository = new JsonTicketRepository(filePath);

            var allTickets = await reopenedRepository.GetAllAsync();

            // Assert: båda ärendena finns kvar.
            Assert.Equal(2, allTickets.Count);

            var restoredFirst = Assert.Single(allTickets, ticket => ticket.Id == firstTicket.Id);

            Assert.Equal(firstTicket.CustomerId, restoredFirst.CustomerId);
            Assert.Equal(firstTicket.Title, restoredFirst.Title);
            Assert.Equal(firstTicket.Description, restoredFirst.Description);
            Assert.Equal(firstTicket.CreatedAt, restoredFirst.CreatedAt);

            Assert.Equal(TicketStatus.InProgress, restoredFirst.Status);
            Assert.Equal(TicketPriority.High, restoredFirst.Priority);
            Assert.Equal(
                technician.TechnicianId,
                restoredFirst.AssignedTechnicianId);

            // Kommentaren behåller både innehåll, ID och datum.
            var restoredComment = Assert.Single(restoredFirst.Comments);

            Assert.Equal(expectedComment.Id, restoredComment.Id);
            Assert.Equal(firstTicket.Id, restoredComment.TicketId);
            Assert.Equal(expectedComment.Comment, restoredComment.Comment);
            Assert.Equal(expectedComment.CreatedAt, restoredComment.CreatedAt);

            var restoredSecond = Assert.Single(allTickets, ticket => ticket.Id == secondTicket.Id);

            Assert.Equal(secondTicket.Title, restoredSecond.Title);
            Assert.Equal(secondTicket.CreatedAt, restoredSecond.CreatedAt);
            Assert.Equal(TicketStatus.New, restoredSecond.Status);
            Assert.Equal(TicketPriority.Low, restoredSecond.Priority);
            Assert.Null(restoredSecond.AssignedTechnician);
            Assert.Empty(restoredSecond.Comments);
        }
        finally
        {
            // Ta bort testets tillfälliga filer.
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, recursive: true);
            }
        }
    }
}