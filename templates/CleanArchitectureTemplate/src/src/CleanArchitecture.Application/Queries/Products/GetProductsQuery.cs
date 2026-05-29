using CleanArchitecture.Application.Contracts.Products;
using CleanArchitecture.Utility;

namespace CleanArchitecture.Application.Queries.Products;

public sealed record GetProductsQuery(
    string? SearchTerm,
    string? SortColum,
    string? SortOrder,
    int PageIndex,
    int PageSize
) : IQuery<PagedList<ProductResponse>>;