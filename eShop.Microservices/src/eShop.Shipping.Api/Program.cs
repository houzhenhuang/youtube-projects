using eShop.Orders.PublicApi;
using eShop.Shipping.Api;
using eShop.Shipping.Api.BackgroundServices;
using eShop.Shipping.Api.Services;
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

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<ShipmentRecordScheduledConsumer>();

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

builder.Services.AddDbContext<ShippingDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"),
        x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "shipping"))
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddHostedService<PendingShipmentScannerService>();

builder.Services.AddHttpClient("orders", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Orders:Url"]!);
});

builder.Services.AddTransient<IOrderService, OrderService>();

builder.WebHost.UseSentry(options =>
{
    options.Dsn = builder.Configuration["Sentry:Dsn"];
    options.SendDefaultPii = true;
    options.SampleRate = 1.0f;
    options.UseOpenTelemetry();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("eShop.Shipping.Api"))
    .WithTracing(tracing =>
    {
        tracing
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddNpgsql()
        .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
    })
    .UseOtlpExporter();

var app = builder.Build();

await app.Services.GetRequiredService<DatabaseInitializer>().Execute();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapShippingEndpoints();

app.Run();