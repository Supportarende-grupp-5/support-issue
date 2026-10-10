using SupportIssue.Application.Customers;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain.Customers;
using SupportIssue.Domain.Entities;

namespace SupportIssue.Tests.UnitTests;

internal class FakeCustomerRepository : ICustomerRepository
{
    public List<Customer> Items { get; } = new();
    public Customer? Updated { get; private set; }
    public void Add(Customer customer) => Items.Add(customer);
    public IReadOnlyList<Customer> GetAll() => Items;
    public Customer? GetById(Guid id) => Items.FirstOrDefault(c => c.Id == id);
    public void Update(Customer customer) => Updated = customer;
}

internal class FakeTicketRepository : ITicketRepository
{
    public List<SupportTicket> Items { get; } = new();
    public Task AddAsync(SupportTicket ticket) { Items.Add(ticket); return Task.CompletedTask; }
    public Task<IReadOnlyList<SupportTicket>> GetAllAsync() => Task.FromResult<IReadOnlyList<SupportTicket>>(Items);
}

internal class FakeHandlingRepository : ITicketHandlingRepository
{
    public SupportTicket Ticket { get; }
    public int Saves { get; private set; }
    public FakeHandlingRepository(SupportTicket ticket) => Ticket = ticket;
    public Task<SupportTicket> GetTicketByIdAsync(Guid ticketId) => Task.FromResult(Ticket);
    public Task<List<SupportTicket>> GetAllTicketsAsync() => Task.FromResult(new List<SupportTicket> { Ticket });
    public Task<bool> SaveTicketAsync(SupportTicket ticket) { Saves++; return Task.FromResult(true); }
}
