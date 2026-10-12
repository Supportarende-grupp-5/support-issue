using SupportIssue.Application.Customers;
using SupportIssue.Application.Models;
using SupportIssue.Application.Services;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain.Customers;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Tests.UnitTests;

public class TicketServiceTests
{
    [Fact]
    public async Task CreateTicketAsync_ValidRequest_StoresTicket()
    {
        var customers = new FakeCustomerRepository();
        var customer = new Customer { Id = Guid.NewGuid(), Name = "A", Email = "a@b.se" };
        customers.Add(customer);
        var tickets = new FakeTicketRepository();
        var service = new TicketService(tickets, customers);

        var ticket = await service.CreateTicketAsync(new CreateTicketRequest
        {
            Title = "T", Description = "D", CustomerId = customer.Id, Priority = TicketPriority.High
        });

        Assert.Same(ticket, Assert.Single(tickets.Items));
        Assert.Equal(TicketPriority.High, ticket.Priority);
    }

    [Fact]
    public async Task CreateTicketAsync_UnknownCustomer_Throws()
    {
        var service = new TicketService(new FakeTicketRepository(), new FakeCustomerRepository());
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTicketAsync(
            new CreateTicketRequest { Title = "T", Description = "D", CustomerId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task CreateTicketAsync_NullRequest_Throws()
    {
        var service = new TicketService(new FakeTicketRepository(), new FakeCustomerRepository());
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateTicketAsync(null!));
    }

    [Fact]
    public void GetCustomers_ReturnsRepositoryCustomers()
    {
        var customers = new FakeCustomerRepository();
        customers.Add(new Customer { Id = Guid.NewGuid() });
        var service = new TicketService(new FakeTicketRepository(), customers);
        Assert.Single(service.GetCustomers());
    }
}

public class CustomerServiceTests
{
    [Fact]
    public void CreateCustomer_Valid_AddsCustomerWithId()
    {
        var repo = new FakeCustomerRepository();
        new CustomerService(repo).CreateCustomer("Anna", "anna@x.se");
        var c = Assert.Single(repo.Items);
        Assert.NotEqual(Guid.Empty, c.Id);
        Assert.Equal("Anna", c.Name);
    }

    [Theory]
    [InlineData("", "a@b.se")]
    [InlineData(" ", "a@b.se")]
    [InlineData("Anna", "")]
    [InlineData("Anna", "no-at-sign")]
    public void CreateCustomer_Invalid_ThrowsAndAddsNothing(string name, string email)
    {
        var repo = new FakeCustomerRepository();
        Assert.Throws<ArgumentException>(() => new CustomerService(repo).CreateCustomer(name, email));
        Assert.Empty(repo.Items);
    }

    [Fact]
    public void UpdateCustomer_Existing_UpdatesFields()
    {
        var repo = new FakeCustomerRepository();
        var c = new Customer { Id = Guid.NewGuid(), Name = "Old", Email = "o@x.se" };
        repo.Add(c);
        new CustomerService(repo).UpdateCustomer(c.Id, "New", "n@x.se");
        Assert.Same(c, repo.Updated);
        Assert.Equal("New", c.Name);
        Assert.Equal("n@x.se", c.Email);
    }

    [Fact]
    public void UpdateCustomer_Missing_Throws()
    {
        var service = new CustomerService(new FakeCustomerRepository());
        Assert.Throws<ArgumentException>(() => service.UpdateCustomer(Guid.NewGuid(), "N", "n@x.se"));
    }

    [Fact]
    public void GetCustomerById_And_GetAll_DelegateToRepository()
    {
        var repo = new FakeCustomerRepository();
        var c = new Customer { Id = Guid.NewGuid() };
        repo.Add(c);
        var service = new CustomerService(repo);
        Assert.Same(c, service.GetCustomerById(c.Id));
        Assert.Null(service.GetCustomerById(Guid.NewGuid()));
        Assert.Single(service.GetAllCustomers());
    }
}

public class TicketHandlingServiceTests
{
    private static (TicketHandlingService svc, FakeHandlingRepository repo) Create()
    {
        var ticket = new SupportTicket("Title", "Desc", Guid.NewGuid(), TicketPriority.Normal);
        var repo = new FakeHandlingRepository(ticket);
        return (new TicketHandlingService(repo), repo);
    }

    [Fact]
    public void GetTechnicians_ReturnsAllFour()
    {
        Assert.Equal(4, Create().svc.GetTechnicians().Count);
    }

    [Fact]
    public async Task AddCommentToTicket_AddsAndSaves()
    {
        var (svc, repo) = Create();
        Assert.True(await svc.AddCommentToTicket(repo.Ticket.Id, "hej"));
        Assert.Single(repo.Ticket.Comments);
        Assert.Equal(1, repo.Saves);
    }

    [Fact]
    public async Task AssignTechnician_Valid_SetsInProgress()
    {
        var (svc, repo) = Create();
        await svc.AssignTechnician(repo.Ticket.Id, 2);
        Assert.Equal(2, repo.Ticket.AssignedTechnicianId);
        Assert.Equal(TicketStatus.InProgress, repo.Ticket.Status);
    }

    [Fact]
    public async Task AssignTechnician_UnknownId_ThrowsWithoutSaving()
    {
        var (svc, repo) = Create();
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AssignTechnician(repo.Ticket.Id, 99));
        Assert.Equal(0, repo.Saves);
    }

    [Fact]
    public async Task CloseTicket_WithoutTechnician_Throws()
    {
        var (svc, repo) = Create();
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CloseTicket(repo.Ticket.Id));
    }

    [Fact]
    public async Task CloseTicket_WithTechnician_Resolves()
    {
        var (svc, repo) = Create();
        await svc.AssignTechnician(repo.Ticket.Id, 1);
        await svc.CloseTicket(repo.Ticket.Id);
        Assert.Equal(TicketStatus.Resolved, repo.Ticket.Status);
    }

    [Theory]
    [InlineData(TicketPriorityOption.Low, TicketPriority.Low)]
    [InlineData(TicketPriorityOption.High, TicketPriority.High)]
    public async Task ChangeTicketPriority_MapsOption(TicketPriorityOption option, TicketPriority expected)
    {
        var (svc, repo) = Create();
        await svc.ChangeTicketPriority(repo.Ticket.Id, option);
        Assert.Equal(expected, repo.Ticket.Priority);
    }

    [Fact]
    public async Task ChangeTicketPriority_Invalid_Throws()
    {
        var (svc, repo) = Create();
        await Assert.ThrowsAsync<ArgumentException>(() => svc.ChangeTicketPriority(repo.Ticket.Id, (TicketPriorityOption)99));
    }

    [Fact]
    public async Task ChangeTicketStatus_InProgressWithoutTechnician_Throws()
    {
        var (svc, repo) = Create();
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ChangeTicketStatus(repo.Ticket.Id, TicketStatusOption.InProgress));
    }

    [Fact]
    public async Task ChangeTicketStatus_Invalid_Throws()
    {
        var (svc, repo) = Create();
        await Assert.ThrowsAsync<ArgumentException>(() => svc.ChangeTicketStatus(repo.Ticket.Id, (TicketStatusOption)99));
    }

    [Fact]
    public async Task ChangeTitleAndDescription_UpdateAndTrim()
    {
        var (svc, repo) = Create();
        await svc.ChangeTicketTitle(repo.Ticket.Id, "  Ny ");
        await svc.ChangeTicketDescription(repo.Ticket.Id, " Beskr ");
        Assert.Equal("Ny", repo.Ticket.Title);
        Assert.Equal("Beskr", repo.Ticket.Description);
    }

    [Fact]
    public async Task GetTicketById_MapsDetailsAndOrdersComments()
    {
        var (svc, repo) = Create();
        await svc.AssignTechnician(repo.Ticket.Id, 3);
        repo.Ticket.AddComment("a");
        repo.Ticket.AddComment("b");

        var d = await svc.GetTicketById(repo.Ticket.Id);

        Assert.Equal(repo.Ticket.Id, d.Id);
        Assert.Equal(TicketStatusOption.InProgress, d.Status);
        Assert.Equal("Sara", d.TechnicianName);
        Assert.Equal(2, d.Comments.Count);
    }
}
