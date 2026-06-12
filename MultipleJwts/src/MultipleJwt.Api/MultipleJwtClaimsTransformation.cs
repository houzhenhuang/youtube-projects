using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MultipleJwt.Api;

public class MultipleJwtClaimsTransformation(IConfiguration configuration) : IClaimsTransformation
{
    private const string AuthSource = "auth_source";
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == AuthSource))
        {
            return Task.FromResult(principal);
        }

        var claimsIdentity = new ClaimsIdentity();

        string? issuer = principal
            .Identities
            .Select(i=>i.FindFirst(JwtRegisteredClaimNames.Iss)?.Value)
            .FirstOrDefault();

        if (issuer == configuration["Authentication:Keycloak:ValidIssuer"])
        {
            claimsIdentity.AddClaim(new Claim(AuthSource, CustomAuthSchemes.Keycloak));
        }
        else if (issuer == configuration["Authentication:Supabase:ValidIssuer"])
        {
            claimsIdentity.AddClaim(new Claim(AuthSource, CustomAuthSchemes.Supabase));
        }

        principal.AddIdentity(claimsIdentity);

        return Task.FromResult(principal);
    }
}