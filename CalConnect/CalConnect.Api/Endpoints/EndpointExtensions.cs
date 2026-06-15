namespace CalConnect.Api.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection service)
    {
        IEnumerable<Type> endpointTypes = typeof(Program).Assembly.GetTypes().Where(a => a is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(a));

        foreach (var endpointType in endpointTypes)
        {
            service.AddTransient(typeof(IEndpoint), endpointType);
        }

        return service;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(app);
        }

        return app;
    }
}