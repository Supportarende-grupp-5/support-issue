using SupportIssue.Domain.Enums;

namespace SupportIssue.Domain.Entities;

public class SupportTicket
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }

    public string? Title { get; private set; }
    public string? Description { get; private set; }

    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public SupportTicket(

        string title,
        string description,
        Guid customerId,
        TicketPriority ticketPriority)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Rubrik måste vara ifylld.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Beskrivning måste vara ifylld.");
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("En kund måste väljas.");
        }

        if (!Enum.IsDefined(Priority))
        {
            throw new ArgumentException("Ogiltig prioritet.");
        }

        Id = Guid.NewGuid();
        CreatedAt = DateTimeOffset.UtcNow;
        Status = TicketStatus.New;

        Title = title.Trim();
        Description = description.Trim();
        CustomerId = customerId;
        Priority = Priority;
    }

}
