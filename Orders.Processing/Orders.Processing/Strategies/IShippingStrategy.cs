namespace Orders.Processing.Strategies;

public interface IShippingStrategy
{
    ShippingProvider ProviderName { get;  }

    decimal CalculateCost(Order order);
}