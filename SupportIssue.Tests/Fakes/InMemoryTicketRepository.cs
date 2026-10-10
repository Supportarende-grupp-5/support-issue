using SupportIssue.Application.Interfaces;
using SupportIssue.Domain.Entities;

namespace SupportIssue.Tests.Fakes
{
    public class InMemoryTicketRepository : ITicketRepository
    {
        private readonly List<SupportTicket> _tickets = new List<SupportTicket>();

        public Task AddAsync(SupportTicket ticket)
        {
            _tickets.Add(ticket);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<SupportTicket>> GetAllAsync()
        {
            IReadOnlyList<SupportTicket> copy = _tickets.ToList();
            return Task.FromResult(copy);
        }
    }
}
