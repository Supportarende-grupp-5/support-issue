namespace SupportIssue.Domain.Entities;

public class TicketComment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TicketComment(Guid id, Guid ticketId, string comment, DateTime createdAt)
    {
        Id = id;
        TicketId = ticketId;
        Comment = comment;
        CreatedAt = createdAt;
    }

    public static TicketComment Restore(Guid id, Guid ticketId, string comment, DateTime createdAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Comment id is required.", nameof(id));
        if (ticketId == Guid.Empty) throw new ArgumentException("Ticket id is required.", nameof(ticketId));
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Comment is required.", nameof(comment));
        if (createdAt == default) throw new ArgumentException("Creation time is required.", nameof(createdAt));
        return new TicketComment(id, ticketId, comment, createdAt);
    }
    
    public TicketComment(Guid ticketId, string comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ArgumentNullException(nameof(comment));
        }
        Id = Guid.NewGuid();
        TicketId = ticketId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
