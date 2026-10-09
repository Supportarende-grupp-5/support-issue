using SupportIssue.Application.TicketHandling;
using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;
using System.Diagnostics.Contracts;

namespace SupportIssue.Tests.UnitTests;

public class SupportTicketTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_InvalidTitle_ThrowsArgumentException(
        string title)

    {
        // Arrange
        var description = "An error appears when logging in.";
        var customerId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
           new SupportTicket(
               title,
               description,
               customerId,
               TicketPriority.Normal));
    }
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Construtor_InvalidDescription_ThrowsArgumentException(
        string description)
    {
        
        Assert.Throws<ArgumentException>(() =>
        new SupportTicket(
            "Cannot log in",
            description,
            Guid.NewGuid(),
            TicketPriority.Normal));  
    }

    [Fact]
    public void Constructor_EmptyCustomerId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
        new SupportTicket(
            "Cannot log in",
            "An error appears when logging in.",
            Guid.Empty,
            TicketPriority.Normal));
    }

    [Fact]
    public void Constructor_InvalidPriority_ThrowsArgumentException()
    {
        var invalidPriority = (TicketPriority)999;

        Assert.Throws<ArgumentException>(() =>
            new SupportTicket(
                "Cannot log in",
                "An error appears when logging in.",
                Guid.NewGuid(),
                invalidPriority));
    }


    [Fact]
    public void CreateTicketAsync_Should_BeCreatedWithStatusNew_ReturnTicketWithStatusNew()
    { 
        //Arrange
        var title = "Cannot log in";
        var description = "An error appears when logging in.";
        var customerId = Guid.NewGuid();
        var priority = TicketPriority.Normal;

        //Act
        var ticket = new SupportTicket(title, description, customerId, priority);

        //Assert
        Assert.Equal(TicketStatus.New, ticket.Status);
    }

    [Fact]
    public void UpdateStatus_Should_ThrowException_When_TechnicianNotAssigned()
    {
        //Arrange 
        var title = "Cannot log in";
        var description = "An error appears when logging in.";
        var customerId = Guid.NewGuid();
        var priority = TicketPriority.Normal;
        var ticket = new SupportTicket(title, description, customerId, priority);


        //Act
        var exception = Assert.Throws<InvalidOperationException>(() => ticket.UpdateStatus(TicketStatus.InProgress));

        // Assert
        Assert.Equal("You may not change status to In Progress without assigning a technician", exception.Message);
    }
    [Fact]
    public void UpdateStatus_Should_ReturnTrue_When_TechnicianAssigned()
    {
        //Arrange 
        var title = "Cannot log in";
        var description = "An error appears when logging in.";
        var customerId = Guid.NewGuid();
        var priority = TicketPriority.Normal;
        var ticket = new SupportTicket(title, description, customerId, priority);
        Technician technician = new Technician("Anna" , 1);
        ticket.AssignTechnician(technician);
    
        //Act
        ticket.UpdateStatus(TicketStatus.InProgress);

        //Assert
        Assert.Equal(TicketStatus.InProgress, ticket.Status);


    }
}
