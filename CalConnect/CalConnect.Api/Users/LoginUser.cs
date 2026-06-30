using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using CalConnect.Api.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class LoginUser(ApplicationDbContext context, PasswordHasher passwordHasher, TokenProvider tokenProvider)
{
    public sealed record Request(string Email, string Password);

    public sealed record Response(string AccessToken, string RefreshToken);

    public async Task<Response> Handle(Request request)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !user.EmailVerified)
        {
            throw new Exception("用户不存在");
        }

        bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!verified)
        {
            throw new Exception("密码错误");
        }

        string token = await tokenProvider.Create(user);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
        };

        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        return new Response(token, refreshToken.Token);
    }
}

public class LoginUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("api/users/login", async (LoginUser.Request request, LoginUser useCase) =>
            {
                var user = await useCase.Handle(request);

                return Results.Ok(user);
            })
            .WithTags(UserEndpoints.Tag);
    }
}