namespace SupportIssue.Domain;

public class TicketComment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public TicketComment(Guid ticketId, string comment)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
