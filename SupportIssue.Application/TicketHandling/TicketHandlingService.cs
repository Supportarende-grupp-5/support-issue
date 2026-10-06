using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Application.TicketHandling;

public class TicketHandlingService(ITicketHandlingRepository ticketHandlingRepository) : ITicketHandlingService
{
    public IReadOnlyList<TechnicianOption> GetTechnicians() => Technician.CreateTechnicianList()
        .Select(item => new TechnicianOption(item.TechnicianId
            ?? throw new InvalidOperationException("Handläggarens id saknas."),
            item.TechnicianName ?? "Namn saknas")).ToList();

    public async Task<bool> AddCommentToTicket(Guid ticketId, string commentText)
    {
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.AddComment(commentText);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public async Task<bool> AssignTechnician(Guid ticketId, int technicianId)
    {
        var technician = Technician.CreateTechnicianList()
            .FirstOrDefault(item => item.TechnicianId == technicianId)
            ?? throw new ArgumentException("Välj en giltig handläggare.", nameof(technicianId));
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.AssignTechnician(technician);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public async Task<bool> CloseTicket(Guid ticketId)
    {
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.Close();
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public async Task<bool> ChangeTicketPriority(Guid ticketId, TicketPriorityOption newPriority)
    {
        var priority = newPriority switch
        {
            TicketPriorityOption.Low => TicketPriority.Low,
            TicketPriorityOption.Normal => TicketPriority.Normal,
            TicketPriorityOption.High => TicketPriority.High,
            _ => throw new ArgumentException("Ogiltig prioritet.", nameof(newPriority))
        };
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.UpdatePriority(priority);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public async Task<bool> ChangeTicketStatus(Guid ticketId, TicketStatusOption newStatus)
    {
        var status = newStatus switch
        {
            TicketStatusOption.New => TicketStatus.New,
            TicketStatusOption.InProgress => TicketStatus.InProgress,
            TicketStatusOption.Resolved => TicketStatus.Resolved,
            _ => throw new ArgumentException("Ogiltig status.", nameof(newStatus))
        };
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.UpdateStatus(status);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }

    public async Task<TicketDetails> GetTicketById(Guid ticketId)
    {
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        return new TicketDetails(ticket.Id, ticket.CustomerId, ticket.Title, ticket.Description,
            ticket.Priority switch
            {
                TicketPriority.Low => TicketPriorityOption.Low,
                TicketPriority.Normal => TicketPriorityOption.Normal,
                TicketPriority.High => TicketPriorityOption.High,
                _ => throw new InvalidOperationException("Ogiltig prioritet.")
            },
            ticket.Status switch
            {
                TicketStatus.New => TicketStatusOption.New,
                TicketStatus.InProgress => TicketStatusOption.InProgress,
                TicketStatus.Resolved => TicketStatusOption.Resolved,
                _ => throw new InvalidOperationException("Ogiltig status.")
            },
            ticket.CreatedAt, ticket.AssignedTechnicianId, ticket.AssignedTechnician?.TechnicianName,
            ticket.Comments.OrderBy(item => item.CreatedAt)
                .Select(item => new TicketCommentDetails(item.Id, item.Comment, item.CreatedAt)).ToList());
    }
    public async Task<bool> ChangeTicketTitle(Guid ticketId, string title)
    {
        var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
        ticket.UpdateTitle(title);
        return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }
    public async Task<bool> ChangeTicketDescription(Guid ticketId, string description)
    {
            var ticket = await ticketHandlingRepository.GetTicketByIdAsync(ticketId);
            ticket.UpdateDescription(description);
            return await ticketHandlingRepository.SaveTicketAsync(ticket);
    }
}
