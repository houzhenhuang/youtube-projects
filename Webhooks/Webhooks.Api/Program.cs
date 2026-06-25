using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Data;
using Webhooks.Api.Extensions;
using Webhooks.Api.Models;
using Webhooks.Api.OpenTelemetry;
using Webhooks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<WebhooksDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks")));

builder.Services.AddScoped<WebhookDispatcher>();

builder.Services.AddHostedService<WebhookProcessor>();

builder.Services.AddSingleton(_ =>
{
    return Channel.CreateBounded<WebhookDispatch>(new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DiagnosticConfig.Source.Name));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

// https://webhook.site/  这个站点用于测试webhook注册

app.MapPost("webhooks/subscriptions", async (
    CreateWebhookRequest request,
    WebhooksDbContext dbContext) =>
{
    var subscription = new WebhookSubscription(
        Guid.NewGuid(),
        request.EventType,
        request.WebhookUrl,
        DateTime.UtcNow);

    dbContext.WebhookSubscriptions.Add(subscription);

    await dbContext.SaveChangesAsync();

    return Results.Ok(subscription);
});

app.MapPost("/orders", async (
        CreateOrderRequest request,
        WebhooksDbContext dbContext,
        WebhookDispatcher webhookDispatcher) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);

    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();

    await webhookDispatcher.DispatchAsync("order.created", order);

    return Results.Ok(order);
})
.WithTags("Orders");

app.MapGet("/orders", async (WebhooksDbContext dbContext) =>
    {
        return Results.Ok(await dbContext.Orders.ToListAsync());
    })
    .WithTags("Orders");

app.Run();
