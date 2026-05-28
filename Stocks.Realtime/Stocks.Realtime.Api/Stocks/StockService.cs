using Dapper;
using Npgsql;
using Stocks.Realtime.Api.Realtime;

namespace Stocks.Realtime.Api.Stocks;

internal sealed class StockService(
    ActiveTickerManager activeTickerManager,
    NpgsqlDataSource dataSource,
    StocksClient stocksClient,
    ILogger<StockService> logger)
{
    public async Task<StockPriceResponse?> GetLatestStockPrice(string ticker)
    {
        try
        {
            // 首先，尝试从数据库获取最后的价格
            StockPriceResponse? dbPrice = await GetLatestPriceFromDatabase(ticker);
            if (dbPrice is not null)
            {
                activeTickerManager.AddTicker(ticker);
                return dbPrice;
            }

            // 如果数据库没有，则从外部API获取
            StockPriceResponse? apiPrice = await stocksClient.GetDataForTicker(ticker);
            if (apiPrice is null)
            {
                logger.LogWarning("根据股票代码：{Ticker} 从外部API没有获取到数据", ticker);
                return null;
            }

            // 保存新价格到数据库
            await SavePriceToDatabase(apiPrice);

            activeTickerManager.AddTicker(ticker);

            return apiPrice;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取股票股票价格时出错：: {Ticker}", ticker);
            throw;
        }
    }

    private async Task<StockPriceResponse?> GetLatestPriceFromDatabase(string ticker)
    {
        const string sql =
            """
            SELECT ticker, price, timestamp
            FROM public.stock_prices
            WHERE ticker = @Ticker
            ORDER BY timestamp DESC
            LIMIT 1
            """;

        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        StockPriceRecord? result = await connection.QueryFirstOrDefaultAsync<StockPriceRecord>(sql, new
        {
            Ticker = ticker
        });

        if (result is not null)
        {
            return new StockPriceResponse(result.Ticker, result.Price);
        }

        return null;
    }
    private async Task SavePriceToDatabase(StockPriceResponse price)
    {
        const string sql =
           """
            INSERT INTO public.stock_prices (ticker, price, timestamp)
            VALUES (@Ticker, @Price, @Timestamp)
            """;

        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            price.Ticker,
            price.Price,
            Timestamp = DateTime.UtcNow
        });
    }

    private sealed record StockPriceRecord(string Ticker, decimal Price, DateTime Timestamp);
}
