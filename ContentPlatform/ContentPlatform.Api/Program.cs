using Carter;
using ContentPlatform.Api.Articles;
using ContentPlatform.Api.Database;
using ContentPlatform.Api.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')));
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddEventBus(options =>
{
    options.UseRabbitMQ(configure =>
    {
        Uri address = new(builder.Configuration.GetConnectionString("RabbitMQ")!);
        var userInfo = address.UserInfo.Split(":");

        configure.UserName = userInfo[0]; // "guest"; // ÕË»§
        configure.Password = userInfo[1]; //"guest"; // ÃÜÂë
        configure.VirtualHost = "/"; // ÐéÄâÖ÷»ú
        configure.HostName = address.Host; //"contentplatform-mq"; //builder.Configuration.GetConnectionString("RabbitMQ")!;
    });
});

builder.Services.AddServiceDiscovery(o=>o.UseConsul());

builder.Services.AddHttpClient<GetReportingArticle.Client>(client =>
{
    client.BaseAddress = new Uri("http://contentplatform-reporting-service");
})
.AddServiceDiscovery()
.AddRoundRobinLoadBalancer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.ApplyMigrations();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();

