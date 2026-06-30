namespace Webhooks.Api.Services;

/// <summary>
/// 
/// </summary>
/// <param name="SubscriptionId"></param>
/// <param name="EventType"></param>
/// <param name="WebhookUrl"></param>
/// <param name="Data"></param>
internal sealed record WebhookTriggered(Guid SubscriptionId, string EventType, string WebhookUrl, object Data);