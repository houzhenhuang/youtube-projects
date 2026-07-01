using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Webhooks.Contracts;
using Webhooks.Processing.Data;
using Webhooks.Processing.Models;

namespace Webhooks.Processing.Services;

internal sealed class WebhookDispatchedConsumer(WebhooksDbContext dbContext) : IConsumer<WebhookDispatched>
{
    public async Task Consume(ConsumeContext<WebhookDispatched> context)
    {
        var message = context.Message;

        var subscriptions = await dbContext.WebhookSubscriptions
           .AsNoTracking()
           .Where(w => w.EventType == message.EventType)
           .ToListAsync();

        foreach (WebhookSubscription subscription in subscriptions)
        {
            await context.Publish(new WebhookTriggered(
                subscription.Id,
                subscription.EventType,
                subscription.WebhookUrl,
                message.Data));
        }

        // 也可以用批量发布
        //await context.PublishBatch(subscriptions.Select(s => new WebhookTriggered(
        //        s.Id,
        //        s.EventType,
        //        s.WebhookUrl,
        //        message.Data)));
    }
}
