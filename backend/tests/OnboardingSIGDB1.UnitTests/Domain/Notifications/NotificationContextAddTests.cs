using FluentAssertions;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.UnitTests.Domain.Notifications;

public class NotificationContextAddTests
{
    [Fact]
    public void AddNotification_AddsNewNotification_And_MakesIsValidFalse()
    {
        var ctx = new NotificationContext();
        ctx.IsValid.Should().BeTrue();
        ctx.AddNotification("Key", "Message");
        ctx.IsValid.Should().BeFalse();
        ctx.Notifications.Should().ContainSingle(n => n.Key == "Key" && n.Message == "Message");
    }

    [Fact]
    public void AddNotification_Duplicate_IsIgnored()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("KeyX", "MsgX");
        ctx.AddNotification("KeyX", "MsgX");
        ctx.Notifications.Should().HaveCount(1);
    }
    
    [Theory]
    [InlineData("", "Message")]
    [InlineData("Key", "")]
    public void AddNotification_ReturnWhenKeyOrMessageIsNull_NoAddMessage(string key, string message)
    {
        var ctx = new NotificationContext();
        ctx.AddNotification(key, message);
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }

    [Fact]
    public void AddNotification_SameKeyDifferentMessage_ShouldAddNewNotification()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("KeyA", "Message1");
        ctx.AddNotification("KeyA", "Message2"); 
        ctx.Notifications.Should().HaveCount(2);
        ctx.Notifications.Should().Contain(n => n.Key == "KeyA" && n.Message == "Message1");
        ctx.Notifications.Should().Contain(n => n.Key == "KeyA" && n.Message == "Message2");
    }

    [Fact]
    public void AddNotification_SameMessageDifferentKey_ShouldAddNewNotification()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("Key1", "SharedMessage");
        ctx.AddNotification("Key2", "SharedMessage");
        ctx.Notifications.Should().HaveCount(2);
        ctx.Notifications.Should().Contain(n => n.Key == "Key1" && n.Message == "SharedMessage");
        ctx.Notifications.Should().Contain(n => n.Key == "Key2" && n.Message == "SharedMessage");
    }

    [Fact]
    public void AddNotification_ExactDuplicate_IsIgnored()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("KeyX", "MsgX");
        ctx.AddNotification("KeyX", "MsgX"); 
        ctx.Notifications.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(null, "Message")]
    [InlineData("Key", null)]
    [InlineData(null, null)]
    [InlineData("   ", "Message")]
    [InlineData("Key", "   ")]
    public void AddNotification_NullOrWhitespaceKeyOrMessage_ShouldNotAdd(string key, string message)
    {
        var ctx = new NotificationContext();
        ctx.AddNotification(key, message);
        ctx.Notifications.Should().BeEmpty();
        ctx.IsValid.Should().BeTrue();
    }
}

