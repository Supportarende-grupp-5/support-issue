namespace SupportIssue.Domain;

public class Ticket
{
    public Guid Id { get; private set; }
    public string TicketTitle { get; set; } =string.Empty;
    public Guid CustomerId { get; set; }
    public string TicketDescription { get; set; } = string.Empty;
    public int? TechnicianId { get; set; }
    public enum TicketStatus
    {
        New,
        InProgress,
        Resolved
    }
    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Low;

}
