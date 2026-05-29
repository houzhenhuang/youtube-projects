using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

namespace CleanArchitecture.Application.Commands.Products;

public sealed record UpdateProductCommand(
    ProductId ProductId,
    string Name,
    string Sku,
    string Currency,
    decimal Amount) : ICommand;