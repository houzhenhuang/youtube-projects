namespace Notification.Sending.Factory;

public interface INotificationSender
{
    NotificationChannel Channel { get; }

    Task SendAsync(NotificationMessage message);
}

public sealed record NotificationMessage(string Recipient, string Subject, string Body);