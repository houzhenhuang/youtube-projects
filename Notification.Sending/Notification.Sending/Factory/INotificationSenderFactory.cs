namespace Notification.Sending.Factory;

public interface INotificationSenderFactory
{
    INotificationSender CreateSender(NotificationChannel channel);
}