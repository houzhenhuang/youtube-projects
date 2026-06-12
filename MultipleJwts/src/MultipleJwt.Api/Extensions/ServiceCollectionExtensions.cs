using Microsoft.OpenApi;

namespace MultipleJwt.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));

            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        Scopes = new Dictionary<string, string>
                        {
                            {"openid","openid"},
                            {"profile","profile"}
                        }
                    }
                }
            });

            o.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference("Keycloak", document)
                ] = []
            });
        });

        return services;
    }
}