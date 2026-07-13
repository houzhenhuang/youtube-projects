namespace eShop.Orders.PublicApi;

public record ReserveOrderStockResult(ReserveOrderStockStatus Status);

public enum ReserveOrderStockStatus
{
    Failed = 0,
    Successful = 1,
}
