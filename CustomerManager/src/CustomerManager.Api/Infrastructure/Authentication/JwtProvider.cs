using CustomerManager.Api.Application.Abstracts.Authentication;
using CustomerManager.Api.Features.Users.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CustomerManager.Api.Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly IClaimsProvider _claimsProvider;

    public JwtProvider(IOptions<JwtOptions> options, IClaimsProvider claimsProvider)
    {
        _options = options.Value;
        _claimsProvider = claimsProvider;
    }
    public async Task<AccessTokens> GetAccessTokens(User user)
    {
        string token = await CreateToken(user);

        RefreshToken refreshToken = CreateRefreshToken();

        return new AccessTokens(token, refreshToken);
    }

    private async Task<string> CreateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        DateTime tokenExpirationTime = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationInMinutes);

        IEnumerable<Claim> claims = await _claimsProvider.GetClaimsForUser(user);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            tokenExpirationTime,
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }

    private RefreshToken CreateRefreshToken()
    {
        var refreshTokenBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(refreshTokenBytes);

        return new RefreshToken(
            Convert.ToBase64String(refreshTokenBytes),
            DateTime.UtcNow.AddMinutes(_options.RefreshTokenExpirationInMinutes));
    }
}