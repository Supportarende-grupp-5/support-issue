using System;
using System.Collections.Generic;
using System.Text;

namespace SupportIssue.Application.TicketOverview
{
    public class TicketStatusCounts
    {
        public int NewCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }
        public int TotalCount { get; set; }
    }
}
