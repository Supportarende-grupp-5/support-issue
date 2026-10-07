using SupportIssue.Domain.Enums;

namespace SupportIssue.Domain.Entities;

public partial class SupportTicket
{
    public void AssignTechnician(Technician technician)
    {
        if (technician == null) throw new ArgumentNullException((nameof(technician)));
        if (technician.TechnicianId is null or <= 0)
            throw new ArgumentException("A technician must have a valid id.", nameof(technician));
        this.AssignedTechnician = technician;
        this.Status = TicketStatus.InProgress;
    }

    public void Close()
    {
        UpdateStatus(TicketStatus.Resolved);
    }

    public void UpdateStatus(TicketStatus status)
    {
        switch (status)
        {
            case TicketStatus.New:
                this.Status = status;
                break;
            case TicketStatus.InProgress:
                if (this.AssignedTechnician == null)
                {
                    throw new InvalidOperationException("You may not change status to In Progress without assigning a technician");
                }
                this.Status = status;
                break;
            case TicketStatus.Resolved:
                if (this.AssignedTechnician == null)
                    throw new InvalidOperationException("You may not change status to Resolved without assigning a technician");
                this.Status = status;
                break;
            default: throw new InvalidOperationException("Invalid status");
        }
    }
    public void UpdatePriority(TicketPriority ticketPriority)
    {
        if (!Enum.IsDefined(ticketPriority))
        {
            throw new InvalidOperationException("Invalid priority");
        }
        else
        {
            this.Priority = ticketPriority;

        }
    }
    public void AddComment(string commentText)
    {
        TicketComment comment = new TicketComment(this.Id, commentText);
        Comments.Add(comment);
    }
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Rubrik måste anges.", nameof(title));

        Title = title.Trim();
    }
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or whitespace.", nameof(description));
        }
        Description = description.Trim();
    }
}
