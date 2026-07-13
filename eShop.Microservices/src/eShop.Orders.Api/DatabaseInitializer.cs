using Dapper;
using Npgsql;

namespace eShop.Orders.Api;

public class DatabaseInitializer(NpgsqlDataSource npgsqlDataSource, IConfiguration configuration, ILogger<DatabaseInitializer> logger)
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("开始初始化数据库。");

            await EnsureDatabaseExists();
            await InitializeDatabase();

            logger.LogInformation("数据库初始化完成。");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库初始化失败。");
        }
    }

    private async Task EnsureDatabaseExists()
    {
        string connectionString = configuration.GetConnectionString("Database")!;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        string? databaseName = builder.Database;
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync();

        bool databaseExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @databaseName)",
            new { databaseName });

        if (!databaseExists)
        {
            logger.LogInformation("开始创建数据库 {DatabaseName}", databaseName);
            await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
        }
    }

    private async Task InitializeDatabase()
    {
        const string sql =
            """
            -- Create schemas if they not't exists
            CREATE SCHEMA IF NOT EXISTS orders;

            -- Create the Orders table if it doesn't exist
            CREATE TABLE IF NOT EXISTS orders.orders (
                id UUID PRIMARY KEY,
                customer_name VARCHAR(255) NOT NULL,
                shipping_address VARCHAR(255) NOT NULL,
                total_price DECIMAL(18, 2) NOT NULL,
                order_date TIMESTAMP WITH TIME ZONE NOT NULL
            );

            CREATE TABLE IF NOT EXISTS orders.order_items (
                id UUID PRIMARY KEY,
                order_id UUID NOT NULL REFERENCES orders.orders(id),
                product_id INTEGER NOT NULL,
                product_name VARCHAR(255) NOT NULL,
                unit_price DECIMAL(18, 2) NOT NULL,
                quantity INTEGER NOT NULL
            );
            """;

        using var connection = await npgsqlDataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql);
    }
}
