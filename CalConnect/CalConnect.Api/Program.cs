using CalConnect.Api.Database;
using CalConnect.Api.Extensions;
using CalConnect.Api.Users;
using CalConnect.Api.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.CustomSchemaIds(id => id.FullName!.Replace("+", "-")));

// UseSnakeCaseNamingConvention 是 EF Core（Entity Framework Core）中的一个扩展方法，主要用于 PostgreSQL 等数据库。
// 它会自动把你的 C# 类名和属性名（PascalCase）转换成 snake_case（下划线命名法），应用到数据库的：表名（Table Name）、列名（Column Name）、外键名、索引名等
// C# 默认使用 PascalCase（如 UserAccount、FullName、CreatedAt）
// PostgreSQL 社区习惯使用 snake_case（如 user_account、full_name、created_at）
builder.Services.AddDbContext<ApplicationDbContext>(o=>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Database")).UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<EmailVerificationLinkFactory>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddFluentEmail(builder.Configuration["Email:SenderEmail"], builder.Configuration["Email:Sender"])
    .AddSmtpSender(builder.Configuration["Email:Host"],builder.Configuration.GetValue<int>("Email:Port"));

builder.Services.AddScoped<RegisterUser>();
builder.Services.AddScoped<LoginUser>();
builder.Services.AddScoped<VerifyEmail>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

UserEndpoints.Map(app);

app.Run();
