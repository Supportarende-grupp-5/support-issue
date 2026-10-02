namespace SupportIssue.Infrastructure.TicketHandling;

public class TicketCommentStorageModel
{
    public Guid Id { get; init; }
    public Guid TicketId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
