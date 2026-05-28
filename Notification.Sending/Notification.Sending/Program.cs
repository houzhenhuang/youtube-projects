using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Sending;
using Notification.Sending.Factory;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<SlackSettings>(builder.Configuration.GetSection("Slack"));

builder.Services.AddSingleton<INotificationSender, EmailNotificationSender>();
builder.Services.AddSingleton<INotificationSender, SmsNotificationSender>();
builder.Services.AddSingleton<INotificationSender, SlackNotificationSender>();

builder.Services.AddSingleton<INotificationSenderFactory, NotificationSenderFactory>();

//// KEYED SERVICES
//builder.Services.AddKeyedSingleton<INotificationSender, EmailNotificationSender>(NotificationChannel.Email);
//builder.Services.AddKeyedSingleton<INotificationSender, SmsNotificationSender>(NotificationChannel.Sms);
//builder.Services.AddKeyedSingleton<INotificationSender, SlackNotificationSender>(NotificationChannel.Slack);

//builder.Services.AddSingleton<INotificationSenderFactory, KeyedNotificationSenderFactory>();

builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

List<NotificationRequest> requests = new List<NotificationRequest>
{
    new (NotificationChannel.Email,"recipient@example.com","Subject","Body")
};

using var scope = app.Services.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

foreach (var request in requests)
{
    Console.WriteLine($"✅  Sending [{request.Channel}] -> {request.Recipient} ...");

    try
    {
        await notificationService.SendAsync(
            request.Channel,
            request.Recipient,
            request.Subject ?? string.Empty,
            request.Body);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌  {ex.GetType().Name}: {ex.Message}");
    }
}

