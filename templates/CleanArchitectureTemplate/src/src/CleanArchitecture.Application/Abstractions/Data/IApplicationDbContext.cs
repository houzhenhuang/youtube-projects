using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    /// <summary>
    /// 产品
    /// </summary>
    DbSet<Product> Products { get; }
}