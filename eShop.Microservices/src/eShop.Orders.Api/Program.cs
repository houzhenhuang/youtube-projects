using eShop.Orders.Api;
using eShop.Orders.Api.Infrastructure;
using eShop.Orders.Api.Orders;
using eShop.Orders.PublicApi;
using eShop.Products.PublicApi;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentry.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("Queue"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton(_ =>
{
    return new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Database")).Build();
});

builder.Services.AddDbContext<OrdersDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders"))
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<IOrderService, OrderService>();

// Replace previous AddHttpClient<IProductService...>
builder.Services.AddHttpClient("products", client =>
{
    var productsUrl = builder.Configuration["Products:Url"];
    if (string.IsNullOrEmpty(productsUrl))
    {
        // Consider throwing a more specific configuration exception
        throw new InvalidOperationException("Products:Url configuration is missing.");
    }
    client.BaseAddress = new Uri(productsUrl);
});
builder.Services.AddTransient<IProductService, ProductService>();

builder.WebHost.UseSentry(options =>
{
    options.Dsn = builder.Configuration["Sentry:Dsn"];
    options.SendDefaultPii = true;
    options.SampleRate = 1.0f;
    options.UseOpenTelemetry();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("eShop.Orders.Api"))
    .WithTracing(tracing =>
    {
        tracing
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddNpgsql()
        .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
        .AddSentry();
    })
    .UseOtlpExporter();

var app = builder.Build();

await app.Services.GetRequiredService<DatabaseInitializer>().Execute();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOrdersEndpoints();

app.Run();