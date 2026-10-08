using System;
using System.Collections.Generic;
using System.Text;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.TicketOverview
{
    public class TicketListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public TicketStatus Status { get; set; }
        public string StatusText { get; set; } = "";
        public TicketPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
