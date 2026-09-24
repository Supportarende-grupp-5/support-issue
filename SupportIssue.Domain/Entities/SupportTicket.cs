using System;
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
        Guid CustomerId,
        TicketPriority ticketPriority)
    {

    }
  
        











}
