using FluentAssertions;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.UnitTests.Domain.Notifications;

public class NotificationTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldSetKeyAndMessage()
    {
        var notification = new Notification("Key", "Message");

        notification.Key.Should().Be("Key");
        notification.Message.Should().Be("Message");
    }

    [Fact]
    public void Constructor_WithNullKey_ShouldDefaultToEmpty()
    {
        var notification = new Notification(null!, "Message");

        notification.Key.Should().Be(string.Empty);
        notification.Message.Should().Be("Message");
    }

    [Fact]
    public void Constructor_WithNullMessage_ShouldDefaultToEmpty()
    {
        var notification = new Notification("Key", null!);

        notification.Key.Should().Be("Key");
        notification.Message.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithBothNull_ShouldDefaultBothToEmpty()
    {
        var notification = new Notification(null!, null!);

        notification.Key.Should().Be(string.Empty);
        notification.Message.Should().Be(string.Empty);
    }
}