using Npgsql;
using Stocks.Realtime.Api;
using Stocks.Realtime.Api.Realtime;
using Stocks.Realtime.Api.Stocks;

// SignalR Examples

// 股票实时数据 API 程序入口点
var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddSingleton(_ =>
{
    string connectionString = builder.Configuration.GetConnectionString("Database")!;

    var npgsqlDataSource = NpgsqlDataSource.Create(connectionString);

    return npgsqlDataSource;
});
builder.Services.AddHostedService<DatabaseInitializer>();

builder.Services.AddHttpClient<StocksClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["Stocks:ApiUrl"]!);
});

builder.Services.AddScoped<StockService>();
builder.Services.AddSingleton<ActiveTickerManager>();
builder.Services.AddHostedService<StocksFeedUpdater>();

builder.Services.Configure<StockUpdateOptions>(builder.Configuration.GetSection("StockUpdateOptions"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.UseCors(policy => policy
    .WithOrigins(builder.Configuration["Cors:AllowedOrigin"]!)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
}

app.MapGet("/api/stocks/{ticker}", async (string ticker, StockService stockService) =>
{
    StockPriceResponse? result = await stockService.GetLatestStockPrice(ticker);

    return result is null ?
    Results.NotFound($"股票代码：{ticker} 没有股票数据可用") :
    Results.Ok(result);
})
.WithName("GetLatestStockPrice");

app.MapHub<StocksFeedHub>("/stocks-feed");

app.UseHttpsRedirection();

app.Run();
