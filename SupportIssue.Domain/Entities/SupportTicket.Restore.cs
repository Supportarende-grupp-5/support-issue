using SupportIssue.Domain.Enums;

namespace SupportIssue.Domain.Entities;

public partial class SupportTicket
{
    private SupportTicket(Guid id, Guid customerId, string title, string description,
        TicketPriority priority, TicketStatus status, DateTimeOffset createdAt,
        Technician? technician, List<TicketComment> comments)
    {
        Id = id;
        CustomerId = customerId;
        Title = title;
        Description = description;
        Priority = priority;
        Status = status;
        CreatedAt = createdAt;
        AssignedTechnician = technician;
        Comments = comments;
    }

    public static SupportTicket Restore(Guid id, Guid customerId, string title,
        string description, TicketPriority priority, TicketStatus status,
        DateTimeOffset createdAt, Technician? technician, IEnumerable<TicketComment> comments)
    {
        if (id == Guid.Empty) throw new ArgumentException("Ticket id is required.", nameof(id));
        if (customerId == Guid.Empty) throw new ArgumentException("A customer must be selected.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (!Enum.IsDefined(priority)) throw new ArgumentException("Invalid priority.", nameof(priority));
        if (!Enum.IsDefined(status)) throw new ArgumentException("Invalid status.", nameof(status));
        if (createdAt == default) throw new ArgumentException("Creation time is required.", nameof(createdAt));
        if (technician != null && technician.TechnicianId is null or <= 0)
            throw new ArgumentException("A technician must have a valid id.", nameof(technician));
        if (status != TicketStatus.New && technician == null)
            throw new InvalidOperationException("In Progress and Resolved tickets require a technician.");

        ArgumentNullException.ThrowIfNull(comments);
        var restoredComments = comments.ToList();
        if (restoredComments.Any(comment => comment == null || comment.TicketId != id))
            throw new ArgumentException("Comments must belong to this ticket.", nameof(comments));

        return new SupportTicket(id, customerId, title, description, priority, status,
            createdAt, technician, restoredComments);
    }
}
