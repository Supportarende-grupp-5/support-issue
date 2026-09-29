namespace SupportIssue.Domain.Entities;

public class TicketComment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
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
