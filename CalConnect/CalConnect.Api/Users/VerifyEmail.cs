using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class VerifyEmail(ApplicationDbContext context)
{
    public async Task<bool> Handle(Guid tokenId)
    {
        EmailVerificationToken? token = await context.EmailVerificationTokens
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == tokenId);

        if (token is null || token.ExpiresOnUtc < DateTime.UtcNow || token.User.EmailVerified)
        {
            return false;
        }

        token.User.EmailVerified = true;

        context.EmailVerificationTokens.Remove(token);

        await context.SaveChangesAsync();

        return true;
    }
}

public class VerifyEmailEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users/verify-email", async (Guid token, VerifyEmail verifyEmail) =>
            {
                bool success = await verifyEmail.Handle(token);

                return success ? Results.Ok() : Results.BadRequest("Invalid or expired token.");
            })
            .WithTags(UserEndpoints.Tag)
            .WithName(UserEndpoints.VerifyEmail);

    }
}