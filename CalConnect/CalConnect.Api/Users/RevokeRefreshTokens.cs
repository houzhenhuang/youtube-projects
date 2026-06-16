using System.Security.Claims;
using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class RevokeRefreshTokens(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
{
    public async Task<bool> Handle(Guid userId)
    {
        if (userId != GetCurrentUserId())
        {
            throw new ApplicationException("You can't do this");
        }
        await context.RefreshTokens
            .Where(r => r.UserId == userId)
            .ExecuteDeleteAsync();

        return true;
    }

    private Guid? GetCurrentUserId()
    {
        return Guid.TryParse(
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid parsed) ? parsed : null;
    }

    internal sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/users/{id:guid}/refresh-tokens", async (Guid id, RevokeRefreshTokens useCase) =>
            {
                bool success = await useCase.Handle(id);

                return success ? Results.NoContent() : Results.BadRequest();
            })
            .WithTags(UserEndpoints.Tag)
            .RequireAuthorization();
        }
    }
}