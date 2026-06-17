using Webhooks.Api.Models;
using Webhooks.Api.Repositories;

namespace Webhooks.Api.Services;

/// <summary>
/// webhook 调度器
/// </summary>
internal sealed class WebhookDispatcher(
    HttpClient httpClient,
    InMemoryWebhookSubscriptionRepository subscriptionRepository)
{
    public async Task DispatchAsync(string eventType, object payload)
    {
        IReadOnlyList<WebhookSubscription> subscriptions = subscriptionRepository.GetByEventType(eventType);

        foreach (WebhookSubscription subscription in subscriptions)
        {
            var request = new
            {
                Id = Guid.NewGuid(),
                subscription.EventType,
                SubscriptionId = subscription.Id,
                Timestamp = DateTime.UtcNow,
                Data = payload
            };

            await httpClient.PostAsJsonAsync(subscription.WebhookUrl, request);
        }
    }
}