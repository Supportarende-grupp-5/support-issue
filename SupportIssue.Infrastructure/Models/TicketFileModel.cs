using SupportIssue.Domain.Enums;

namespace SupportIssue.Infrastructure.Models;

public class TicketFileModel
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TicketPriority TicketPriority { get; set; }
    public TicketStatus TicketStatus { get; set; }

    public DateTimeOffset CreatedAt { get; set; }


}
