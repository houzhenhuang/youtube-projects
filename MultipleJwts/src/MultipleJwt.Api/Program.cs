using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MultipleJwt.Api;
using MultipleJwt.Api.Extensions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(CustomAuthSchemes.Keycloak, options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = builder.Configuration["Authentication:Keycloak:Audience"];
        options.MetadataAddress = builder.Configuration["Authentication:Keycloak:MetadataAddress"]!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:Keycloak:ValidIssuer"]
        };
    })
    .AddJwtBearer(CustomAuthSchemes.Supabase, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new JsonWebKey(File.ReadAllText("supabase.json")),
            ValidAudience = builder.Configuration["Authentication:Supabase:ValidAudience"],
            ValidIssuer = builder.Configuration["Authentication:Supabase:ValidIssuer"]
        };
    });
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicy defaultPolicy =
        new AuthorizationPolicyBuilder(CustomAuthSchemes.Keycloak, CustomAuthSchemes.Supabase)
            .RequireAuthenticatedUser()
            .Build();

    options.DefaultPolicy = defaultPolicy;
});

builder.Services.AddTransient<IClaimsTransformation, MultipleJwtClaimsTransformation>();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService("MultipleJwt.Api");
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        tracing.AddConsoleExporter();
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
