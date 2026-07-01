using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Webhooks.Api.Data;
using Webhooks.Api.Models;
using Webhooks.Api.OpenTelemetry;
using Webhooks.Contracts;

namespace Webhooks.Api.Services;

/// <summary>
/// webhook 调度器
/// </summary>
internal sealed class WebhookDispatcher(IPublishEndpoint publishEndpoint)
{
    public async Task DispatchAsync<T>(string eventType, T data)
        where T : notnull
    {
        using Activity? activity = DiagnosticConfig.Source.StartActivity($"{eventType} 调度 webhook");
        activity?.AddTag("event.type", eventType);

        await publishEndpoint.Publish(new WebhookDispatched(eventType, data));
    }
}