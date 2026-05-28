using CustomerManager.Api.Application.Abstracts.Authentication;
using CustomerManager.Api.Endpoints;
using CustomerManager.Api.Features.Users.Entities;
using CustomerManager.Api.Infrastructure.Authentication;
using CustomerManager.Api.Metrics;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenApiSamples.Data;
using System.Text.Json;

namespace CustomerManager.Api.Features.Authentication;

public static class Login
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email">邮箱</param>
    /// <param name="Password">密码</param>
    public sealed record LoginRequest(string Email, string Password);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Token">token</param>
    /// <param name="RefreshToken">refresh token</param>
    /// <param name="RefreshTokenExpiresOnUtc">refresh token 过期时间</param>
    public sealed record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiresOnUtc);

    public sealed class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {
            RuleFor(r => r.Email).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder authGroup = app.MapGroup("api/authentication").WithTags("认证");

            authGroup.MapPost("/login", Handler).WithSummary("登录")
                //.Produces<TokenResponse>(contentType: "application/json; charset=utf-8")
                ;
        }

        public async Task<IResult> Handler(LoginRequest input, IJwtProvider jwtProvider, AppDbContext dbContext, ILogger<Endpoint> logger, CustomerMetrics customerMetrics)
        {
            using var _ = customerMetrics.MeasureRequestDuration();

            try
            {
                logger.LogInformation($"开始登录，入参：{JsonSerializer.Serialize(input)}");

                var user = await dbContext.Set<User>().FirstOrDefaultAsync(x => x.Email == input.Email && x.PasswordHash == input.Password);
                if (user is null)
                {
                    return Results.BadRequest("用户名或密码不正确");
                }

                AccessTokens accessTokens = await jwtProvider.GetAccessTokens(user);

                return Results.Ok(accessTokens.CreateTokenResponse());

            }
            finally
            {
                customerMetrics.IncreaseUserLoginReuqestCount();
            }
        }
    }
}

public static class TokenResponseExtensions
{
    public static Login.TokenResponse CreateTokenResponse(this AccessTokens accessTokens)
    {
        return new Login.TokenResponse(accessTokens.Token, accessTokens.RefreshToken.Token, accessTokens.RefreshToken.ExpiresOnUtc);
    }
}
