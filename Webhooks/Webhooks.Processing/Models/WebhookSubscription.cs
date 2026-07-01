namespace Webhooks.Processing.Models;

/// <summary>
/// webhook 订阅
/// </summary>
/// <param name="Id"></param>
/// <param name="EventType"></param>
/// <param name="WebhookUrl"></param>
/// <param name="CreateOnUtc"></param>
public sealed record WebhookSubscription(Guid Id, string EventType, string WebhookUrl, DateTime CreateOnUtc);

public sealed record CreateWebhookRequest(string EventType, string WebhookUrl);