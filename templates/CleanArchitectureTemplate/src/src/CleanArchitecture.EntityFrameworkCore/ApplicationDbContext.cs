using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Domain.Primitives;
using CleanArchitecture.EntityFrameworkCore.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.EntityFrameworkCore;

public class ApplicationDbContext : DbContext, IUnitOfWork, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Product> Products => Set<Product>();

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await _mediator.DispatchDomainEventsAsync(this);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}