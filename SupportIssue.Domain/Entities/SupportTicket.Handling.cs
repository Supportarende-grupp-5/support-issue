using SupportIssue.Domain.Enums;

namespace SupportIssue.Domain.Entities;

public partial class SupportTicket
{
    public void AssignTechnician(Technician technician)
    {
        if (technician == null) throw new ArgumentNullException((nameof(technician)));
        this.AssignedTechnician=technician;
    }
    public void UpdateStatus(TicketStatus status)
    {
        switch(status)
        {
            case TicketStatus.New:
                    this.Status = status;
                break;
            case TicketStatus.InProgress:
                if (this.AssignedTechnician == null)
                {
                    throw new InvalidOperationException("You may not change status to In Progress without assigning a technician");
                }
                this .Status = status;
                break;
            case TicketStatus.Resolved:
                if (this.AssignedTechnician == null)
                {
                    throw new InvalidOperationException("You may not change status to Resolved without assigning a technician");
                }
                this.Status = status;
                break;
            default: throw new InvalidOperationException("Invalid status");
        }
    }
}
