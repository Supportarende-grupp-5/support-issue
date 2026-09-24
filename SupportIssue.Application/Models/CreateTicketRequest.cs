using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Models;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } 
    public Guid CustomerId { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Normal;

}
