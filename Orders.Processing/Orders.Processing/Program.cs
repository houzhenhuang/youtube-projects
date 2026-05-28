using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Processing;
using Orders.Processing.Strategies;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddTransient<OrderProcessor>();

builder.Services.AddTransient<IShippingStrategy, FedExShippingStrategy>();
builder.Services.AddTransient<IShippingStrategy, UpsShippingStrategy>();
builder.Services.AddTransient<IShippingStrategy, UspsShippingStrategy>();
builder.Services.AddTransient<IShippingStrategy, DhlShippingStrategy>();

builder.Services.AddTransient<IUspsApi, UspsApi>();

PrintHeader("Statement Model");

var app = builder.Build();

var processor = app.Services.GetRequiredService<OrderProcessor>();

var lightOrder = new Order { Id = 1, CustomerName = "John Doe", TotalWeight = 10.5m };

Console.WriteLine($"Light order ({lightOrder.TotalWeight} KG)");
foreach (var provider in Enum.GetValues<ShippingProvider>())
{
    Console.WriteLine($"  {provider,-6} : ${processor.CalculateShippingCost(lightOrder, provider):F2}");
}

Console.WriteLine();

static void PrintHeader(string title)
{
    Console.WriteLine($"⌈ {title} {new string('-', Math.Max(0, 60 - title.Length))} ⌉");
}
