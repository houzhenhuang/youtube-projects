using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.WebApi;

namespace Notification.Sending.Factory;

public class SlackNotificationSender(IOptions<SlackSettings> slackSettings) : INotificationSender
{
    private readonly SlackSettings _slackSettings = slackSettings.Value;
    public NotificationChannel Channel => NotificationChannel.Slack;

    public async Task SendAsync(NotificationMessage message)
    {
        var slackClient = new SlackApiClient(_slackSettings.BotToken);
        await slackClient.Chat.PostMessage(new Message
        {
            Channel = message.Recipient,
            Text = message.Body
        });
    }
}


public sealed class SlackSettings
{
    public string BotToken { get; set; } = "xoxb-demo-token";
}