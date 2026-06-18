using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Data;
using Webhooks.Api.Models;

namespace Webhooks.Api.Services;

/// <summary>
/// webhook 调度器
/// </summary>
internal sealed class WebhookDispatcher(
    IHttpClientFactory httpClientFactory,
    WebhooksDbContext dbContext)
{
    public async Task DispatchAsync<T>(string eventType, T data)
    {
        var subscriptions = await dbContext.WebhookSubscriptions
            .AsNoTracking()
            .Where(w => w.EventType == eventType)
            .ToListAsync();

        foreach (WebhookSubscription subscription in subscriptions)
        {
            using var httpClient = httpClientFactory.CreateClient();

            var payload = new WebhookPayload<T>
            {
                Id = Guid.NewGuid(),
                EventType = subscription.EventType,
                SubscriptionId = subscription.Id,
                Timestamp = DateTime.UtcNow,
                Data = data
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            try
            {
                var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload);

                var attempt = new WebhookDeliveryAttempt
                {
                    Id = Guid.NewGuid(),
                    WebhookSubscriptionId = subscription.Id,
                    Payload = jsonPayload,
                    ResponseStatusCode = (int)response.StatusCode,
                    Success = response.IsSuccessStatusCode,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.WebhookDeliveryAttempts.Add(attempt);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var attempt = new WebhookDeliveryAttempt
                {
                    Id = Guid.NewGuid(),
                    WebhookSubscriptionId = subscription.Id,
                    Payload = jsonPayload,
                    ResponseStatusCode = null,
                    Success = false,
                    Timestamp = DateTime.UtcNow
                };

                dbContext.WebhookDeliveryAttempts.Add(attempt);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}