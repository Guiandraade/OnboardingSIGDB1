using FluentAssertions;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.UnitTests.Domain.Notifications;

public class NotificationContextStateTests
{
    [Fact]
    public void IsValid_ShouldBeFalse_WhenThereAreMultipleNotifications()
    {
        var entity = new NotificationContext();
        entity.AddNotification("Erro 2", "Erro 2");
        var result = entity.IsValid;
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldBeTrue_WhenNoNotificationsExist()
    {
        var ctx = new NotificationContext();
        ctx.IsValid.Should().BeTrue();
        ctx.Notifications.Should().BeEmpty();
    }

    [Fact]
    public void Clear_RemovesAllNotifications_And_IsValidBecomesTrue()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("Key", "Message");
        ctx.Clear();
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Notifications_ShouldBeReadOnly()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("K", "M");
        ctx.Notifications.Should().BeAssignableTo<IReadOnlyCollection<Notification>>();
        var act = () => ((ICollection<Notification>)ctx.Notifications).Add(new Notification("X", "Y"));
        act.Should().Throw<NotSupportedException>();
    }
}

