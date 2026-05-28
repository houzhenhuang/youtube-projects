using Microsoft.Extensions.DependencyInjection;

namespace Notification.Sending.Factory;

public class NotificationSenderFactory : INotificationSenderFactory
{
    private readonly IDictionary<NotificationChannel, INotificationSender> _senders;
    public NotificationSenderFactory(IEnumerable<INotificationSender> senders)
    {
        _senders = senders.ToDictionary(s => s.Channel, s => s);
    }
    public INotificationSender CreateSender(NotificationChannel channel)
    {
        if (!_senders.TryGetValue(channel, out var sender))
        {
            throw new ArgumentOutOfRangeException(nameof(channel), $"No sender registered for channel {channel}");
        }

        return sender;
    }
}

public class KeyedNotificationSenderFactory(IServiceProvider serviceProvider) : INotificationSenderFactory
{
    public INotificationSender CreateSender(NotificationChannel channel)
    {
        var sender = serviceProvider.GetRequiredKeyedService<INotificationSender>(channel);

        return sender;
    }
}