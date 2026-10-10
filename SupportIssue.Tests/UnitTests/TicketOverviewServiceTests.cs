using SupportIssue.Application.TicketOverview;
using SupportIssue.Domain;
using SupportIssue.Domain.Customers;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using SupportIssue.Tests.Fakes;

namespace SupportIssue.Tests.UnitTests
{
    public class TicketOverviewServiceTests
    {
        private readonly InMemoryTicketRepository _ticketRepository = new InMemoryTicketRepository();
        private readonly InMemoryCustomerRepository _customerRepository = new InMemoryCustomerRepository();
        private readonly TicketOverviewService _service;

        private readonly Customer _anna = new Customer { Id = Guid.NewGuid(), Name = "Anna Andersson", Email = "anna@test.se" };
        private readonly Customer _bertil = new Customer { Id = Guid.NewGuid(), Name = "Bertil Berg", Email = "bertil@test.se" };

        public TicketOverviewServiceTests()
        {
            _customerRepository.Add(_anna);
            _customerRepository.Add(_bertil);

            _service = new TicketOverviewService(_ticketRepository, _customerRepository);
        }

        private async Task<SupportTicket> AddTicket(string title, Customer customer)
        {
            var ticket = new SupportTicket(title, "Test description", customer.Id, TicketPriority.Normal);
            await _ticketRepository.AddAsync(ticket);
            return ticket;
        }
        [Fact]
        public async Task Search_TitleWithDifferentCase_FindsTicket()
        {
            // Arrange
            await AddTicket("Printer not working", _anna);
            await AddTicket("Cannot log in", _bertil);

            // Act
            var result = await _service.GetTicketsAsync("PRINTER", null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Printer not working", result[0].Title);
        }

        [Fact]
        public async Task Search_CustomerName_FindsTicket()
        {
            await AddTicket("Printer not working", _anna);
            await AddTicket("Cannot log in", _bertil);

            var result = await _service.GetTicketsAsync("bertil", null);

            Assert.Single(result);
            Assert.Equal("Bertil Berg", result[0].CustomerName);
        }
        [Fact]
        public async Task Filter_ByStatus_ReturnsOnlyThatStatus()
        {
            // Arrange: one New, one InProgress, one Resolved
            await AddTicket("New ticket", _anna);

            var inProgress = await AddTicket("Ongoing ticket", _anna);
            inProgress.AssignTechnician(new Technician("Anna", 1));

            var resolved = await AddTicket("Done ticket", _bertil);
            resolved.AssignTechnician(new Technician("Johan", 2));
            resolved.Close();

            // Act
            var result = await _service.GetTicketsAsync("", TicketStatus.Resolved);

            // Assert
            Assert.Single(result);
            Assert.Equal(TicketStatus.Resolved, result[0].Status);
        }

        [Fact]
        public async Task SearchAndFilter_Together_ReturnsOnlyMatchingTicket()
        {
            await AddTicket("Printer not working", _anna);

            var resolvedPrinter = await AddTicket("Printer out of paper", _bertil);
            resolvedPrinter.AssignTechnician(new Technician("Sara", 3));
            resolvedPrinter.Close();

            var result = await _service.GetTicketsAsync("printer", TicketStatus.New);

            Assert.Single(result);
            Assert.Equal("Printer not working", result[0].Title);
        }
        [Fact]
        public async Task GetStatusCounts_CountsEachStatus()
        {
            await AddTicket("Ticket 1", _anna);
            await AddTicket("Ticket 2", _anna);

            var ongoing = await AddTicket("Ticket 3", _bertil);
            ongoing.AssignTechnician(new Technician("Erik", 4));

            var counts = await _service.GetStatusCountsAsync();

            Assert.Equal(2, counts.NewCount);
            Assert.Equal(1, counts.InProgressCount);
            Assert.Equal(0, counts.ResolvedCount);
            Assert.Equal(3, counts.TotalCount);
        }
    }
}
