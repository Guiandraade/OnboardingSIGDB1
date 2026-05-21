using FluentAssertions;
using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.UnitTests.Domain.Notifications;

public class NotificationContextAddRangeTests
{
    [Fact]
    public void AddRange_WithValidationFailures_AddAllMessages()
    {
        var ctx = new NotificationContext();
        var failures = new[]
        {
            new ValidationFailure("PropA", "ErrA"),
            new ValidationFailure("PropB", "ErrB"),
        };
        ctx.AddRange(failures);
        ctx.Notifications.Should().HaveCount(failures.Length);
        ctx.Notifications.Select(n => n.Key).Should().Contain(new[] { "PropA", "PropB" });
    }

    [Fact]
    public void AddRange_ReturnWhenNotificationsIsNull()
    {
        var ctx = new NotificationContext();
        IEnumerable<Notification>? failures = null;
        ctx.AddRange(failures!);
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void AddRange_AddSomenteWhenNotificationsIsDiferenteDeNull()
    {
        var ctx = new NotificationContext();
        IEnumerable<Notification> failures = new[]
        {
            new Notification("KeyA", "MsgA"),
            new Notification("KeyB", "MsgB"),
            new Notification(null!, null!)
        };
        ctx.AddRange(failures);
        ctx.Notifications.Should().HaveCount(2);
        ctx.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public void AddRange_ReturnWhenNotificationsFailureIsNull()
    {
        var ctx = new NotificationContext();
        IEnumerable<ValidationFailure>? failures = null;
        ctx.AddRange(failures!);
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }

    [Fact]
    public void AddRange_Notifications_WithNullItemInList_ShouldSkipNullAndAddValid()
    {
        var ctx = new NotificationContext();
        var list = new List<Notification>
        {
            new("A", "MsgA"),
            null!,
            new("B", "MsgB"),
        };
        ctx.AddRange(list);
        ctx.Notifications.Should().HaveCount(2);
        ctx.Notifications.Should().Contain(n => n.Key == "A");
        ctx.Notifications.Should().Contain(n => n.Key == "B");
    }

    [Fact]
    public void AddRange_Notifications_EmptyList_ShouldNotAddAny()
    {
        var ctx = new NotificationContext();
        ctx.AddRange(Enumerable.Empty<Notification>());
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }

    [Fact]
    public void AddRange_ValidationFailures_EmptyList_ShouldNotAddAny()
    {
        var ctx = new NotificationContext();
        ctx.AddRange(Enumerable.Empty<ValidationFailure>());
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }
}

