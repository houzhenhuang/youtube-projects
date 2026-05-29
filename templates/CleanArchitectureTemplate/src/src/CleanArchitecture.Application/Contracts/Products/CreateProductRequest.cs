namespace CleanArchitecture.Application.Contracts.Products;

/// <summary>
/// 
/// </summary>
/// <param name="Name"></param>
/// <param name="Sku"></param>
/// <param name="Currency"></param>
/// <param name="Amount"></param>
public sealed record CreateProductRequest(
    string Name,
    string Sku,
    string Currency,
    decimal Amount
);