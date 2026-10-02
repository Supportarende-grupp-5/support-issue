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
}
