using SupportIssue.Domain;
using SupportIssue.Domain.Entities;
using SupportIssue.Domain.Enums;

namespace SupportIssue.Tests.UnitTests;

public class DomainTests
{
    private static SupportTicket NewTicket() => new("T", "D", Guid.NewGuid(), TicketPriority.Normal);

    [Fact]
    public void Constructor_Valid_TrimsAndSetsDefaults()
    {
        var t = new SupportTicket("  T ", " D ", Guid.NewGuid(), TicketPriority.Low);
        Assert.Equal("T", t.Title);
        Assert.Equal("D", t.Description);
        Assert.Equal(TicketStatus.New, t.Status);
        Assert.Empty(t.Comments);
    }

    [Fact]
    public void Constructor_EmptyCustomerOrBadPriority_Throws()
    {
        Assert.Throws<ArgumentException>(() => new SupportTicket("T", "D", Guid.Empty, TicketPriority.Low));
        Assert.Throws<ArgumentException>(() => new SupportTicket("T", "D", Guid.NewGuid(), (TicketPriority)99));
    }

    [Fact]
    public void AssignTechnician_Null_OrInvalidId_Throws()
    {
        var t = NewTicket();
        Assert.Throws<ArgumentNullException>(() => t.AssignTechnician(null!));
        Assert.Throws<ArgumentException>(() => t.AssignTechnician(new Technician("X", 0)));
        Assert.Throws<ArgumentException>(() => t.AssignTechnician(new Technician("X", null)));
    }

    [Fact]
    public void UpdateStatus_NewAlwaysAllowed_ResolvedNeedsTechnician()
    {
        var t = NewTicket();
        t.UpdateStatus(TicketStatus.New);
        Assert.Throws<InvalidOperationException>(() => t.UpdateStatus(TicketStatus.Resolved));
        Assert.Throws<InvalidOperationException>(() => t.UpdateStatus((TicketStatus)99));
    }

    [Fact]
    public void UpdatePriority_Invalid_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => NewTicket().UpdatePriority((TicketPriority)99));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateTitleAndDescription_Blank_Throws(string value)
    {
        var t = NewTicket();
        Assert.Throws<ArgumentException>(() => t.UpdateTitle(value));
        Assert.Throws<ArgumentException>(() => t.UpdateDescription(value));
    }

    [Fact]
    public void AddComment_Blank_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => NewTicket().AddComment(" "));
    }

    [Fact]
    public void Restore_ValidAndInvalid()
    {
        var id = Guid.NewGuid();
        var created = DateTimeOffset.UtcNow;
        var t = SupportTicket.Restore(id, Guid.NewGuid(), "T", "D", TicketPriority.Normal,
            TicketStatus.InProgress, created, new Technician("Anna", 1), Array.Empty<TicketComment>());
        Assert.Equal(id, t.Id);

        Assert.Throws<InvalidOperationException>(() => SupportTicket.Restore(id, Guid.NewGuid(), "T", "D",
            TicketPriority.Normal, TicketStatus.Resolved, created, null, Array.Empty<TicketComment>()));
        Assert.Throws<ArgumentException>(() => SupportTicket.Restore(Guid.Empty, Guid.NewGuid(), "T", "D",
            TicketPriority.Normal, TicketStatus.New, created, null, Array.Empty<TicketComment>()));
        Assert.Throws<ArgumentException>(() => SupportTicket.Restore(id, Guid.NewGuid(), "T", "D",
            TicketPriority.Normal, TicketStatus.New, created, null, new[] { new TicketComment(Guid.NewGuid(), "x") }));
    }

    [Fact]
    public void TicketComment_Restore_ValidatesInput()
    {
        var c = TicketComment.Restore(Guid.NewGuid(), Guid.NewGuid(), "x", DateTime.UtcNow);
        Assert.Equal("x", c.Comment);
        Assert.Throws<ArgumentException>(() => TicketComment.Restore(Guid.Empty, Guid.NewGuid(), "x", DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => TicketComment.Restore(Guid.NewGuid(), Guid.NewGuid(), " ", DateTime.UtcNow));
        Assert.Throws<ArgumentException>(() => TicketComment.Restore(Guid.NewGuid(), Guid.NewGuid(), "x", default));
    }
}
