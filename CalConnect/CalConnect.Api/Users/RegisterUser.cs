using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using CalConnect.Api.Users.Infrastructure;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CalConnect.Api.Users;

internal class RegisterUser(
    ApplicationDbContext context,
    PasswordHasher passwordHasher,
    IFluentEmail fluentEmail,
    EmailVerificationLinkFactory emailVerificationLinkFactory)
{
    public record Request(string Email, string FirstName, string LastName, string Password);

    public async Task<User> Handle(Request request)
    {
        if (await context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new Exception("当前邮箱已被使用");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHasher.Hash(request.Password)
        };

        context.Users.Add(user);

        DateTime utcNow = DateTime.UtcNow;
        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedOnUtc = utcNow,
            ExpiresOnUtc = utcNow.AddDays(1)
        };

        context.EmailVerificationTokens.Add(verificationToken);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
            when (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new Exception("当前邮箱已被使用");

        }

        // Email verification
        string verificationLink = emailVerificationLinkFactory.Create(verificationToken);

        await fluentEmail
            .To(user.Email)
            .Subject("CalConnect 的电子邮件验证")
            .Body($"要验证您的电子邮件地址<a href='{verificationLink}'>请点击此处</a>", isHtml: true)
            .SendAsync();

        // Access token

        return user;
    }
}

public class RegisterUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("api/users/register", async (RegisterUser.Request request, RegisterUser registerUser) =>
            {
                var user = await registerUser.Handle(request);

                return Results.Ok(user);
            })
            .WithTags(UserEndpoints.Tag);
    }
}