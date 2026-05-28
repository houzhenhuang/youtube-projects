using Notification.Sending.Factory;

namespace Notification.Sending;

public class NotificationService(INotificationSenderFactory factory)
{
    public async Task SendAsync(NotificationChannel channel, string recipient, string subject, string body)
    {
        var sender = factory.CreateSender(channel);
        await sender.SendAsync(new NotificationMessage(recipient, subject, body));
    }

    public async Task SendBulkAsync(IEnumerable<NotificationRequest> requests)
    {
        foreach (var request in requests)
        {
            await SendAsync(request.Channel, request.Recipient, request.Subject ?? string.Empty, request.Body);
        }
    }
}

public sealed record NotificationRequest(NotificationChannel Channel, string Recipient, string? Subject, string Body);