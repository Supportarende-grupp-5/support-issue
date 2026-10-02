using SupportIssue.Domain;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Infrastructure.TicketHandling;

public class TicketStorageModel
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Description { get; init ; } = string.Empty;

    public TicketPriority Priority { get; init; }
    public TicketStatus Status { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
    public int? AssignedTechnicianId { get; init; }
    public List<TicketCommentStorageModel> Comments { get; init; } = [];
}
