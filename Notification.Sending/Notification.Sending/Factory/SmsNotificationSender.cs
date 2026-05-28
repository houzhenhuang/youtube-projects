using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Notification.Sending.Factory;

public class SmsNotificationSender(IOptions<TwilioSettings> twilioSettings) : INotificationSender
{
    private readonly TwilioSettings _twilioSettings = twilioSettings.Value;
    public NotificationChannel Channel => NotificationChannel.Sms;

    public async Task SendAsync(NotificationMessage message)
    {
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
        await MessageResource.CreateAsync(
            to: new PhoneNumber(message.Recipient),
            from: new PhoneNumber(_twilioSettings.FromNumber),
            body: message.Body);
    }
}

public sealed class TwilioSettings
{
    public string AccountSid { get; set; } = "ACdemo";
    public string AuthToken { get; set; } = "demo-token";
    public string FromNumber { get; set; } = "+13565434564";
}