namespace CleanArchitecture.Application.Commands.Products;

/// <summary>
/// 
/// </summary>
/// <param name="RequestId"></param>
/// <param name="Name"></param>
/// <param name="Sku"></param>
/// <param name="Currency"></param>
/// <param name="Amount"></param>
public sealed record CreateProductCommand(
    Guid RequestId,
    string Name,
    string Sku,
    string Currency,
    decimal Amount) : ICommand;