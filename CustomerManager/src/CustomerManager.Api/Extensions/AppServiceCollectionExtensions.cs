using CustomerManager.Api.Abstractions.Behaviors;
using CustomerManager.Api.Application.Abstracts.Authentication;
using CustomerManager.Api.Infrastructure.Authentication;
using CustomerManager.Api.Infrastructure.Authorization.Handlers;
using CustomerManager.Api.Infrastructure.Authorization.Providers;
using CustomerManager.Api.Metrics;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OpenApiSamples.Data;
using OpenApiSamples.Endpoints;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CustomerManager.Api.Extensions;

public static class AppServiceCollectionExtensions
{
    public static IServiceCollection AddServiceDefault(this IServiceCollection services, IHostApplicationBuilder builder)
    {
        AddOpenApi(services);
        AddDatabase(services);
        AddAuthentication(services);
        AddAuthorization(services);
        AddEndpoints(services);
        AddMediatR(services);
        AddValidators(services);
        AddOpenTelemetry(services, builder);
        services.AddHttpClient();
        return services;
    }

    private static void AddOpenApi(IServiceCollection services)
    {
        services.AddOpenApi("openapi");
    }
    private static void AddDatabase(IServiceCollection services)
    {
        //services.AddDbContext<AppDbContext>(options => options.UseSqlite("Filename=app.sqlite"));
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer("Server=customermanager-database;Database=customer-manager;User Id=sa;Password=Strong_password_123!;TrustServerCertificate=true;"));
    }
    private static void AddAuthentication(IServiceCollection services)
    {
        services
          .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer();

        services.ConfigureOptions<JwtOptionsConfigueOptions>();
        services.ConfigureOptions<JwtBearerOptionsConfigureOptions>();

        services.AddTransient<IJwtProvider, JwtProvider>();
        services.AddTransient<IClaimsProvider, ClaimsProvider>();
    }
    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddScoped<IPermissionService, PermissionService>();
    }
    private static void AddEndpoints(IServiceCollection services)
    {
        services.AddEndpoints();
    }

    private static void AddMediatR(IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);

            config.AddOpenBehavior(typeof(ValidatorPipelineBehavior<,>));
        });
    }
    private static void AddValidators(IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
    }
    private static void AddOpenTelemetry(IServiceCollection services, IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

        const string serviceName = "CustomerManager.Api";
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                //.AddSqlClientInstrumentation(o =>
                //{
                //    o.SetDbStatementForText = true;
                //})
                .AddEntityFrameworkCoreInstrumentation(o => o.SetDbStatementForText = true);
                //.AddRedisInstrumentation()
                //.AddNpgsql();

                tracing.AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                .AddAspNetCoreInstrumentation()
                .AddMeter(CustomerMetrics.MeterName)
                ;

                metrics.AddPrometheusExporter()
                .AddOtlpExporter();
            });

        services.AddMetrics();
        services.AddSingleton<CustomerMetrics>();
    }

    public static IApplicationBuilder UseDefault(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapEndpoints();

        app.MapPrometheusScrapingEndpoint();

        return app;
    }
}
