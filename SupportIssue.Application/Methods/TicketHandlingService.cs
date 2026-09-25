using SupportIssue.Domain;
using SupportIssue.Infrastructure;
using static SupportIssue.Domain.Ticket;

namespace SupportIssue.Application.Methods;

public class TicketHandlingService(ITicketHandlingRepository ticketHandlingRepository) : ITicketHandlingService
{
    public Task<Ticket> AddCommentToTicket(Guid ticketId, string comment)
    {
        try
        {
            var addedComment = ticketHandlingRepository.AddCommentToTicketAsync(ticketId, comment);
            return addedComment;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while adding the comment to the ticket.", ex);
            { }
        }
    }

    public Task<Ticket> AssignTechnician(Guid ticketId, Guid technicianId)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> ChangePriority(Guid ticketId, TicketPriority newPriority)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> ChangeTicketStatus(Guid ticketId, TicketStatus newStatus)
    {
        throw new NotImplementedException();
    }

    public Task<Ticket> EditTicket(Guid technicianId, string ticketTitle, string ticketDescription)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TicketComment>> GetCommentsByTicketId(Guid ticketId)
    {
        try
        {
            var comments = ticketHandlingRepository.GetCommentsByTicketIdAsync(ticketId);
            return comments;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while retrieving the comments.", ex);
        }
    }

    public async Task<Ticket> GetTicketById(Guid ticketId)
    {
        try
        {
            var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
            return ticket;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while retrieving the ticket.", ex);
        }
    }
}
