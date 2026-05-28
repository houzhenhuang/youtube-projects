using Dapper;
using Npgsql;

namespace Stocks.Realtime.Api;

internal sealed class DatabaseInitializer(
    NpgsqlDataSource dataSource,
    IConfiguration configuration,
    ILogger<DatabaseInitializer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("开始数据库初始化。");

            await EnsureDatabaseExists();
            await InitializeDatabase();

            logger.LogInformation("数据库初始化成功完成。");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库初始化出现错误。");
        }
    }

    private async Task EnsureDatabaseExists()
    {
        string connectionString = configuration.GetConnectionString("Database")!;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        string? databaseName = builder.Database;
        builder.Database = "postgres"; // Connect to the default 'postgres' database

        using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync();

        bool databaseExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname = @databaseName)",
            new { databaseName });

        if (!databaseExists)
        {
            logger.LogInformation("Creating database {DatabaseName}",databaseName);
            await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
        }
    }

    private async Task InitializeDatabase()
    {
        // 股票价格表
        const string sql =
            """
            -- Check if the table exists, if not, create it
            CREATE TABLE IF NOT EXISTS public.stock_prices (
                id SERIAL PRIMARY KEY,
                ticker VARCHAR(10) NOT NULL,
                price NUMERIC(12, 6) NOT NULL,
                timestamp TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC')
            );

            -- Create an index on the ticker column for faster lookups
            CREATE INDEX IF NOT EXISTS idx_stock_prices_ticker ON public.stock_prices(ticker);

            -- Create an index on the timestamp column for faster time-based queries
            CREATE INDEX IF NOT EXISTS idx_stock_prices_timestamp ON public.stock_prices(timestamp);
            """;
        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql);
    }
}
