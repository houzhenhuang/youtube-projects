namespace Webhooks.Api.Services;

//internal sealed record WebhookDispatch(string EventType, object Data,string? ParentActivityId);
internal sealed record WebhookDispatched(string EventType, object Data);