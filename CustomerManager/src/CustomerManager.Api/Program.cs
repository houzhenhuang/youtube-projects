using System.Reflection;
using CustomerManager.Api.Extensions;
using CustomerManager.Api.Infrastructure;
using Microsoft.Extensions.Configuration.UserSecrets;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceDefault(builder);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

# region UserSecrets
Console.WriteLine($"»œ÷§key:{builder.Configuration["Authentication:Key"]}");
Console.WriteLine(builder.Environment.ApplicationName);

var appAssembly = Assembly.Load(new AssemblyName(builder.Environment.ApplicationName));
UserSecretsIdAttribute? attribute = appAssembly.GetCustomAttribute<UserSecretsIdAttribute>();
Console.WriteLine(attribute?.UserSecretsId);
# endregion

var app = builder.Build();

app.MapOpenApi();

//if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.ApplySeedData();
}

app.UseDefault();

//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseExceptionHandler();

app.Run();