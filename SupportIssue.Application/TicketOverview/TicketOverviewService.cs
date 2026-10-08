using System;
using System.Collections.Generic;
using System.Text;
using SupportIssue.Application.Customers;
using SupportIssue.Application.Interfaces;
using SupportIssue.Domain.Customers;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.TicketOverview
{
    public class TicketOverviewService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICustomerRepository _customerRepository;

        public TicketOverviewService(
            ITicketRepository ticketRepository,
            ICustomerRepository customerRepository)
        {
            _ticketRepository = ticketRepository;
            _customerRepository = customerRepository;
        }

        // Returns the tickets that match the search text and the status.
        // status == null means "all statuses".
        public async Task<List<TicketListItem>> GetTicketsAsync(string searchText, TicketStatus? status)
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var customers = _customerRepository.GetAll();

            var result = new List<TicketListItem>();

            foreach (var ticket in tickets)
            {
                string customerName = GetCustomerName(customers, ticket.CustomerId);

                if (!MatchesSearch(ticket, customerName, searchText))
                {
                    continue;
                }

                if (status.HasValue && ticket.Status != status.Value)
                {
                    continue;
                }

                result.Add(CreateListItem(ticket, customerName));
            }

            // Newest tickets first
            
            
            return result.OrderByDescending(item => item.CreatedAt).ToList();
        }

        public async Task<TicketStatusCounts> GetStatusCountsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var counts = new TicketStatusCounts();

            foreach (var ticket in tickets)
            {
                if (ticket.Status == TicketStatus.New)
                {
                    counts.NewCount++;
                }
                else if (ticket.Status == TicketStatus.InProgress)
                {
                    counts.InProgressCount++;
                }
                else if (ticket.Status == TicketStatus.Resolved)
                {
                    counts.ResolvedCount++;
                }
            }

            counts.TotalCount = tickets.Count;

            return counts;
        }
        private string GetCustomerName(IReadOnlyList<Customer> customers, Guid customerId)
        {
            foreach (var customer in customers)
            {
                if (customer.Id == customerId)
                {
                    return customer.Name;
                }
            }

            return "Unknown customer";
        }

        private bool MatchesSearch(SupportTicket ticket, string customerName, string searchText)
        {
            // Empty search box = show everything
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return true;
            }

            string text = searchText.Trim();

            if (ticket.Title.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (customerName.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private TicketListItem CreateListItem(SupportTicket ticket, string customerName)
        {
            var item = new TicketListItem();

            item.Id = ticket.Id;
            item.Title = ticket.Title;
            item.CustomerName = customerName;
            item.Status = ticket.Status;
            item.StatusText = GetStatusText(ticket.Status);
            item.Priority = ticket.Priority;
            item.CreatedAt = ticket.CreatedAt.LocalDateTime;

            return item;
        }

        private string GetStatusText(TicketStatus status)
        {
            switch (status)
            {
                case TicketStatus.New:
                    return "New";
                case TicketStatus.InProgress:
                    return "In progress";
                case TicketStatus.Resolved:
                    return "Resolved";
                default:
                    return status.ToString();
            }
        }
    }
}
