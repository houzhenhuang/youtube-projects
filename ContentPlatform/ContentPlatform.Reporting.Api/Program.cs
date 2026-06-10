using Carter;
using ContentPlatform.Reporting.Api.Database;
using ContentPlatform.Reporting.Api.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(id => id.FullName!.Replace('+', '-')));
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    //o.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
    o.UseNpgsql(builder.Configuration.GetConnectionString("contentplatform-db")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddEventBus(options =>
{
    options.UseRabbitMQ(configure =>
    {
        //Uri address = new(builder.Configuration.GetConnectionString("RabbitMQ")!);
        Uri address = new(builder.Configuration.GetConnectionString("contentplatform-mq")!);
        var userInfo = address.UserInfo.Split(":");

        configure.UserName = userInfo[0]; // "guest"; // �˻�
        configure.Password = userInfo[1]; //"guest"; // ����
        configure.VirtualHost = "/"; // ��������
        configure.HostName = address.Host; //"contentplatform-mq"; //builder.Configuration.GetConnectionString("RabbitMQ")!;
    });
});

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(resource => resource.AddService("ContentPlatform.Reporting.Api"))
//    .WithTracing(tracing =>
//    {
//        // 埋点
//        tracing
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            //.AddSqlClientInstrumentation()    // 如果是 PostgreSQL，建议注释掉，避免冲突
//            .AddRabbitMQInstrumentation()
//            .AddEntityFrameworkCoreInstrumentation(options =>
//            {
//                // 可以保留 Enrich 做额外增强
//                options.EnrichWithIDbCommand = (activity, command) =>
//                {
//                    foreach (NpgsqlParameter param in command.Parameters)
//                    {
//                        var value = param.Value?.ToString() ?? "(null)";
//                        activity.SetTag($"db.query.parameter.{param.ParameterName}", value);
//                    }
//                };
//            })
//            .AddNpgsql()
//            ;

//        tracing.AddOtlpExporter();
//    });


builder.Services.AddServiceDiscovery(o => o.UseConsul());

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.ApplyMigrations();
}

app.MapCarter();

app.Run();
