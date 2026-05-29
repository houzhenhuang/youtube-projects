using CleanArchitecture.Application.Contracts.Products;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;

namespace CleanArchitecture.Application.Queries.Products;

/// <summary>
/// 
/// </summary>
public sealed record GetProductQuery(ProductId ProductId) : IQuery<ProductResponse>;