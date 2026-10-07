namespace SupportIssue.Application.TicketHandling;

public enum TicketStatusOption { New, InProgress, Resolved }
public enum TicketPriorityOption { Low, Normal, High }
public record TechnicianOption(int TechnicianId, string TechnicianName);
public record TicketCommentDetails(Guid Id, string Comment, DateTime CreatedAt);
public record TicketDetails(Guid Id, Guid CustomerId, string Title, string Description,
    TicketPriorityOption Priority, TicketStatusOption Status, DateTimeOffset CreatedAt,
    int? AssignedTechnicianId, string? TechnicianName, IReadOnlyList<TicketCommentDetails> Comments);
