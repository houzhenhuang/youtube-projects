using Microsoft.Extensions.Configuration;
using SlackNet;
using SlackNet.WebApi;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Notification.Sending;

public class NotificationServiceBefore
{
    private readonly IConfiguration _config;
    public NotificationServiceBefore(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(NotificationChannel channel, string recipient, string subject, string body)
    {
        if (channel == NotificationChannel.Email)
        {
            using var smtp = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]!));
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]);

            var message = new MailMessage(
                from: _config["Smtp:From"],
                to: recipient,
                subject: subject,
                body: body);
            message.IsBodyHtml = true;

            await smtp.SendMailAsync(message);
        }
        else if (channel == NotificationChannel.Sms)
        {
            TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);
            await MessageResource.CreateAsync(
                to: new PhoneNumber(recipient),
                from: new PhoneNumber(_config["Twilio:FromNumber"]),
                body: body);
        }
        else if (channel == NotificationChannel.Slack)
        {
            var slackClient = new SlackApiClient(_config["Slack:BotToken"]);
            await slackClient.Chat.PostMessage(new Message
            {
                Channel = recipient,
                Text = body
            });
        }
        else
        {
            throw new ArgumentException($"Unsupported channel: {channel}");
        }
    }

    public async Task SendBulkAsync(IEnumerable<NotificationRequestBefore> requests)
    {
        foreach (var request in requests)
        {
            await SendAsync(request.Channel, request.Recipient, request.Subject ?? string.Empty, request.Body);
        }
    }
}

public sealed record NotificationRequestBefore(NotificationChannel Channel, string Recipient, string? Subject, string Body);