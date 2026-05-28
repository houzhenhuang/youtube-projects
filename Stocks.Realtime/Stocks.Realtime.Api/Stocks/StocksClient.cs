using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using static Stocks.Realtime.Api.Stocks.StockPriceRequest.StockPriceRequestData;

namespace Stocks.Realtime.Api.Stocks;

internal sealed class StocksClient(
    HttpClient httpClient,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    ILogger<StocksClient> logger)
{
    public async Task<StockPriceResponse?> GetDataForTicker(string ticker)
    {
        logger.LogInformation("获取股票代码为 {Ticker} 的股票价格信息", ticker);

        StockPriceResponse? stockPriceResponse = await memoryCache.GetOrCreateAsync($"stocks-{ticker}", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            return await GetStockPrice(ticker);
        });

        if (stockPriceResponse is null)
        {
            logger.LogWarning("未能获取 {Ticker} 的股票价格信息", ticker);
        }
        else
        {
            logger.LogInformation("已完成获取 {Ticker}、{@stock}的股票价格信息", ticker, stockPriceResponse);
        }

        return stockPriceResponse;
    }

    private async Task<StockPriceResponse?> GetStockPrice(string ticker)
    {
        try
        {
            StockPriceRequest request = new()
            {
                Trace = Guid.NewGuid().ToString(),
                Data = new StockPriceRequest.StockPriceRequestData()
                {
                    Code = ticker,
                    Type = KlineType.FifteenKline,
                    TimestampEnd = 0,
                    QueryKlineNum = 1,
                    AdjustType = 0
                }
            };

            string jsonString = JsonSerializer.Serialize(request);


            httpClient.BaseAddress = new Uri($"{httpClient.BaseAddress?.OriginalString}/kline");
            string tickerDataString = await httpClient.GetStringAsync(
                $"?token={configuration["Stocks:AllTickApiKey"]}&query={HttpUtility.UrlEncode(jsonString)}");

            //AlphaVantageData? tickerData = JsonSerializer.Deserialize<AlphaVantageData>(tickerDataString);
            AllTickResponse? tickerData = JsonSerializer.Deserialize<AllTickResponse>(tickerDataString);

            KlineItem? lastPrice = tickerData?.Data?.KlineItems?.FirstOrDefault();

            if (lastPrice is null)
            {
                return null;
            }

            return new StockPriceResponse(ticker, decimal.Parse(lastPrice.HighPrice, CultureInfo.InvariantCulture));

        }
        catch (Exception ex)
        {
            logger.LogError("获取股票编号为：{Ticker} 的价格出现异常", ticker);
        }

        return null;
    }
}

