using SupportIssue.Application.Customers;
using SupportIssue.Application.Interfaces;
using SupportIssue.Application.Models;
using SupportIssue.Domain.Customers;
using SupportIssue.Domain.Entities;

namespace SupportIssue.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICustomerRepository _customerRepository;

    public TicketService(
        ITicketRepository ticketRepository,
        ICustomerRepository customerRepository)
    {
        _ticketRepository = ticketRepository;
        _customerRepository = customerRepository;

    }

    public SupportTicket CreateTicket(CreateTicketRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = _customerRepository.GetById(request.CustomerId);

        if (customer is null)
        {
            throw new ArgumentException("The selected customer does not exist");
        }

        var ticket = new SupportTicket(
            request.Title,
            request.Description,
            request.CustomerId,
            request.Priority);

        _ticketRepository.Add(ticket);

        return ticket;
    }

    public IReadOnlyList<Customer> GetCustomers()
    {
        return _customerRepository.GetAll();
    }
}
