using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Webhooks.Contracts;
using Webhooks.Processing.Data;
using Webhooks.Processing.Models;

namespace Webhooks.Processing.Services;

internal sealed class WebhookTriggeredConsumer(
    IHttpClientFactory httpClientFactory,
    WebhooksDbContext dbContext) : IConsumer<WebhookTriggered>
{
    public async Task Consume(ConsumeContext<WebhookTriggered> context)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var payload = new WebhookPayload
        {
            Id = Guid.NewGuid(),
            EventType = context.Message.EventType,
            SubscriptionId = context.Message.SubscriptionId,
            Timestamp = DateTime.UtcNow,
            Data = context.Message.Data
        };

        var jsonPayload = JsonSerializer.Serialize(payload);

        try
        {
            var response = await httpClient.PostAsJsonAsync(context.Message.WebhookUrl, payload);
            response.EnsureSuccessStatusCode();

            var attempt = new WebhookDeliveryAttempt
            {
                Id = Guid.NewGuid(),
                WebhookSubscriptionId = context.Message.SubscriptionId,
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
                WebhookSubscriptionId = context.Message.SubscriptionId,
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
