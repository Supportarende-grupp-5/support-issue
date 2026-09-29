using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Methods;

public class TicketHandlingService(ITicketHandlingRepository ticketHandlingRepository) : ITicketHandlingService
{
    public async Task<bool> AddCommentToTicket(SupportTicket ticket, string comment)
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

    public async Task<bool> AssignTechnician(SupportTicket ticket, int technicianId)
    {
        try
        {
            var technicians = Technician.CreateTechnicianList();

            Technician? technician = null;

            foreach (var item in technicians)
            {
                if (item.TechnicianId == technicianId)
                {
                    technician = item;
                    break;
                }
            }
            if (technician == null) { throw new ArgumentNullException(nameof(technician)); }

            ticket.AssignTechnician(technician);
            return await ticketHandlingRepository.SaveTicketAsync(ticket);
            
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while adding the comment to the ticket.", ex);
        }
    }

    public async Task<SupportTicket> ChangeTicketPriority(SupportTicket ticket, TicketPriority newPriority)
    {
        throw new NotImplementedException();

    }

    public async Task<bool> ChangeTicketStatus(SupportTicket ticket, TicketStatus newStatus)
    {
        ticket.UpdateStatus(newStatus);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public Task<List<TicketComment>> GetCommentsByTicket(SupportTicket ticket)
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

    public async Task<SupportTicket> GetTicketById(Guid ticketId)
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
