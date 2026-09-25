using SupportIssue.Application.Models;
using SupportIssue.Domain.Entities;

namespace SupportIssue.Application.Interfaces;

public interface ITicketService
{
    SupportTicket CreateTicket(CreateTicketRequest request);

}
