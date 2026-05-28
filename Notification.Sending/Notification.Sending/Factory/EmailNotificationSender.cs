using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Notification.Sending.Factory;

public class EmailNotificationSender(IOptions<SmtpSettings> smtpSettings) : INotificationSender
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
    public NotificationChannel Channel => NotificationChannel.Email;

    public async Task SendAsync(NotificationMessage message)
    {
        using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
        smtp.EnableSsl = true;
        smtp.Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

        var mailMessage = new MailMessage(
            from: _smtpSettings.From,
            to: message.Recipient,
            subject: message.Subject,
            body: message.Body);
        mailMessage.IsBodyHtml = true;

        await smtp.SendMailAsync(mailMessage);
    }
}

public sealed class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}