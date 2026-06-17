using Webhooks.Api.Models;
using Webhooks.Api.Repositories;
using Webhooks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();

builder.Services.AddHttpClient<WebhookDispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}

app.UseHttpsRedirection();

// https://webhook.site/  这个站点用于测试webhook注册

app.MapPost("webhooks/subscriptions", (
    CreateWebhookRequest request,
    InMemoryWebhookSubscriptionRepository subscriptionRepository) =>
{
    var subscription = new WebhookSubscription(
        Guid.NewGuid(),
        request.EventType,
        request.WebhookUrl,
        DateTime.UtcNow);

    subscriptionRepository.Add(subscription);

    return Results.Ok(subscription);
});

app.MapPost("/orders", async (
        CreateOrderRequest request,
        InMemoryOrderRepository orderRepository,
        WebhookDispatcher webhookDispatcher) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);

    orderRepository.Add(order);

    await webhookDispatcher.DispatchAsync("order.created", order);

    return Results.Ok(order);
})
.WithTags("Orders");

app.MapGet("/orders", (InMemoryOrderRepository orderRepository) =>
    {

        return Results.Ok(orderRepository.GetAll());
    })
    .WithTags("Orders");

app.Run();
