using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.Methods;

public class TicketHandlingService(ITicketHandlingRepository ticketHandlingRepository) : ITicketHandlingService
{
    public async Task<bool> AddCommentToTicket(SupportTicket ticket, string commentText)
    {
            ticket.AddComment(commentText);
            return await ticketHandlingRepository.SaveTicketAsync(ticket);

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

    public async Task<bool> ChangeTicketPriority(SupportTicket ticket, TicketPriority newPriority)
    {
        ticket.UpdatePriority(newPriority);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);

    }

    public async Task<bool> ChangeTicketStatus(SupportTicket ticket, TicketStatus newStatus)
    {
        ticket.UpdateStatus(newStatus);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public List<TicketComment> GetCommentsByTicket(SupportTicket ticket)
    {            
            return ticket.Comments.OrderBy(x => x.CreatedAt).ToList(); 
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
