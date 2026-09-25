namespace SupportIssue.Domain;

public class TicketComment
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public TicketComment(Guid ticketId, string comment)
    {
        Id = Guid.NewGuid();
        TicketId = ticketId;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
