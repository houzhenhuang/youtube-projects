using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

namespace CleanArchitecture.Application.Commands.Products;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
public sealed record DeleteProductCommand(ProductId ProductId) : ICommand;