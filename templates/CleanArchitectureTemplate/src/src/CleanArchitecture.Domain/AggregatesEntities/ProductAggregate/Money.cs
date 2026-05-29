namespace CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

/// <summary>
/// 
/// </summary>
/// <param name="Currency"></param>
/// <param name="Amount"></param>
public record Money(string Currency, decimal Amount);