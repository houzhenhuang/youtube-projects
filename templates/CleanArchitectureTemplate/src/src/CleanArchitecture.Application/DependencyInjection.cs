using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly)
                // .AddOpenBehavior(typeof(UnitOfWorkBehavior<,>))
                // .AddOpenBehavior(typeof(ValidatorBehavior<,>), ServiceLifetime.Scoped)
                // .AddOpenBehavior(typeof(LoggingBehavior<,>), ServiceLifetime.Scoped)
        );

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}