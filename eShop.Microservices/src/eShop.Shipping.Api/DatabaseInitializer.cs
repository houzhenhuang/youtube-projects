using Dapper;
using Npgsql;

namespace eShop.Shipping.Api;

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
            CREATE SCHEMA IF NOT EXISTS shipping;

            -- Create the shipment_records table if it doesn't exist
            CREATE TABLE IF NOT EXISTS shipping.shipment_records (
                id UUID PRIMARY KEY,
                order_id UUID NOT NULL,
                tracking_number VARCHAR(50) NOT NULL,
                shipping_address VARCHAR(255) NOT NULL,
                created_at TIMESTAMP WITH TIME ZONE NOT NULL,
                status INTEGER NOT NULL,
                CONSTRAINT fk_order FOREIGN KEY (order_id) REFERENCES orders.orders(id)
            );
            """;

        using var connection = await npgsqlDataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql);
    }
}
