using SupportIssue.Domain;
using SupportIssue.Infrastructure;
using System.Xml.Linq;
using static SupportIssue.Domain.Ticket;

namespace SupportIssue.Application.Methods;

public class TicketHandlingService(ITicketHandlingRepository ticketHandlingRepository) : ITicketHandlingService
{
    public async Task<bool> AddCommentToTicket(Ticket ticket, string comment)
    {
        try
        {
            await ticketHandlingRepository.AddCommentToTicketAsync(ticket, comment);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while adding the comment to the ticket.", ex);
        }
    }

    public async Task<bool> AssignTechnician(Ticket ticket, int technicianId)
    {
        try
        {
            await ticketHandlingRepository.AssignTechnicianAsync(ticket, technicianId);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while adding the comment to the ticket.", ex);
        }
    }

    public async Task<Ticket> ChangeTicketPriority(Ticket ticket, TicketPriority newPriority)
    {
        throw new NotImplementedException();

    }

    public async Task<Ticket> ChangeTicketStatus(Ticket ticket, TicketStatus newStatus)
    {
        throw new NotImplementedException();

    }

    public Task<List<TicketComment>> GetCommentsByTicket(Ticket ticket)
    {
        try
        {
            var comments = ticketHandlingRepository.GetCommentsByTicketAsync(ticket);
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
