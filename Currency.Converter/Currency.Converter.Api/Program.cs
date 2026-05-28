using Currency.Converter.Api;
using System.Collections.Concurrent;

// https://www.youtube.com/watch?v=VgU4D8WWEs8
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var currencyApiConfig = builder.Configuration.GetSection("CurrencyApi");
var baseUrl = currencyApiConfig["BaseUrl"] ??
              throw new InvalidOperationException("CurrencyApi:BaseUrl is required in configuration");
var apiKey = currencyApiConfig["ApiKey"] ??
             throw new InvalidOperationException("CurrencyApi:ApiKey is required in configuration");

builder.Services.AddHttpClient<CurrencyApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("apikey", apiKey);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("api/convert/{currencyCode}", async (string currencyCode, decimal amount, CurrencyApiClient currencyApiClient) =>
{
    // 验证货币代码格式（3个大写字母）
    if (string.IsNullOrWhiteSpace(currencyCode) ||
        currencyCode.Length != 3 ||
        !currencyCode.All(char.IsLetter))
    {
        return Results.BadRequest(new { error = "货币代码必须是3个字母的大写代码（例如，EUR, GBP, CNY）" });
    }

    // 验证金额（必须为正）
    if (amount < 0)
    {
        return Results.BadRequest(new { error = "金额必须是正数" });
    }

    decimal? rate = await ExchangeRateHelper.GetExchangeRate(currencyCode, currencyApiClient);

    if (rate is null)
    {
        return Results.NotFound(new { error = $"找不到 {currencyCode} 的汇率或发生API错误" });
    }

    var convertedAmount = amount * rate.Value;

    return Results.Ok(new ExchangeRateResponse(
           Currency: currencyCode,
           BaseCurrency: "USD",
           Rate: rate.Value,
           Amount: amount,
           ConvertedAmount: convertedAmount
       ));
});

app.Run();


public static class ExchangeRateHelper
{
    private record CacheEntry(decimal Rate, DateTime CreatedAtUtc);
    private static readonly ConcurrentDictionary<string, CacheEntry> Cache = [];
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    //private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

    public static async Task<decimal?> GetExchangeRate(string currencyCode, CurrencyApiClient currencyApiClient)
    {
        if (Cache.TryGetValue(currencyCode, out var entry) && IsFresh(entry))
        {
            return entry.Rate;
        }

        var semaphore = Locks.GetOrAdd(currencyCode, _ => new SemaphoreSlim(1, 1));

        bool acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(5));
        if (!acquired)
        {
            throw new TimeoutException("无法获取汇率。稍后再试");
        }

        try
        {
            if (Cache.TryGetValue(currencyCode, out entry) && IsFresh(entry))
            {
                return entry.Rate;
            }

            decimal? currentRate = await currencyApiClient.GetExchangeRateAsync(currencyCode);

            if (currentRate != null)
            {
                Cache.AddOrUpdate(currencyCode,
                    _ => new CacheEntry(currentRate.Value, DateTime.UtcNow),
                    (_, _) => new CacheEntry(currentRate.Value, DateTime.UtcNow));
            }

            return currentRate;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static bool IsFresh(CacheEntry entry)
    {
        return DateTime.UtcNow - entry.CreatedAtUtc < CacheDuration;
    }
}

