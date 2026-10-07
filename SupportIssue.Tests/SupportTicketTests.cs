using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Tests;

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
}
