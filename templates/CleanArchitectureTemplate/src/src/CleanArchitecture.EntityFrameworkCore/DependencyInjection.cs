using CleanArchitecture.Application.Abstractions.Data;
using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.Domain.Primitives;
using CleanArchitecture.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, IConfiguration configuration)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
        
        services.AddDbContext<ApplicationDbContext>(o =>
        {
            o.UseMySql(configuration.GetConnectionString("Default"), serverVersion);
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}