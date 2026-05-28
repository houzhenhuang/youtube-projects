
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Stocks.Realtime.Api.Stocks;

namespace Stocks.Realtime.Api.Realtime;

/// <summary>
/// 股票信息更新后台服务
/// </summary>
internal sealed class StocksFeedUpdater(
    ActiveTickerManager activeTickerManager,
    IServiceScopeFactory serviceScopeFactory,
    IHubContext<StocksFeedHub, IStockUpdateClient> hubContext,
    IOptions<StockUpdateOptions> options,
    ILogger<StocksFeedUpdater> logger) : BackgroundService
{
    private readonly Random _random = new();
    private readonly StockUpdateOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateStockPrices();

            await Task.Delay(_options.UpdateInterval);
        }
    }

    private async Task UpdateStockPrices()
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        StockService stockService = scope.ServiceProvider.GetRequiredService<StockService>();

        foreach (string ticker in activeTickerManager.GetAllTickers())
        {
            StockPriceResponse? currentPrice = await stockService.GetLatestStockPrice(ticker);
            if (currentPrice == null)
            {
                continue;
            }

            decimal newPrice = CalculateNewPrice(currentPrice);

            var update = new StockPriceUpdate(ticker, newPrice);

            //await hubContext.Clients.All.ReceiveStockPriceUpdate(update);

            // 向特定的组接收股票价格更新，这样可以告知那些真正对这支股票代码感兴趣的客户
            await hubContext.Clients.Group(ticker).ReceiveStockPriceUpdate(update);

            logger.LogInformation("更新 {Ticker} 价格为 {Price}", ticker, newPrice);
        }
    }

    private decimal CalculateNewPrice(StockPriceResponse currentPrice)
    {
        double change = _options.MaxPercentageChange;
        // 价格因素
        decimal priceFactor = (decimal)(_random.NextDouble() * change * 2 - change);
        decimal priceChange = currentPrice.Price * priceFactor;
        decimal newPrice = Math.Max(0, currentPrice.Price * priceChange);
        newPrice = Math.Round(newPrice, 2);
        return newPrice;
    }
}