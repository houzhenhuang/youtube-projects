using Carter;
using CleanArchitecture.Api.Extensions;
using CleanArchitecture.Application;
using CleanArchitecture.EntityFrameworkCore;
using CleanArchitecture.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

var configuration = GetConfiguration();

Log.Logger = CreateSerilogLogger(configuration);

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, c) => { c.ReadFrom.Configuration(ctx.Configuration); });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddControllers();

    builder.Services
        .AddApplication()
        .AddInfrastructure()
        .AddEntityFrameworkCore(builder.Configuration);

    builder.Services.AddCarter(new DependencyContextAssemblyCatalog(typeof(Program).Assembly));

    Log.Information("正在创建主机。");

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseLogContextEnrichment();

    app.UseSerilogRequestLogging();

    app.UseGlobalExceptionHandler();

    app.UseRouting();

    app.MapCarter();

    app.MapControllers();

    await app.InitializeAsync();

    Log.Information("应用程序开始启动");

    app.Run();

    Log.Information("应用程序已启动完成");
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序意外终止");
}
finally
{
    Log.CloseAndFlush();
}

ILogger CreateSerilogLogger(IConfiguration config)
{
    return new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();
}

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}