using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using CalConnect.Api.Roles.Domain;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Roles;

internal sealed class GetRoles(ApplicationDbContext context)
{
    public sealed record Response(int Id, string Name);

    public async Task<List<Response>> Handle(CancellationToken cancellationToken)
    {
        return await context.Roles.Select(r => new Response(r.Id, r.Name)).ToListAsync();
    }

    internal sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("api/roles", async (GetRoles useCase, CancellationToken cancellationToken) =>
            {
                List<Response> response = await useCase.Handle(cancellationToken);

                return Results.Ok(response);
            })
            .WithTags(RoleEndpoint.Tag)
            .RequireAuthorization(policy => policy.RequireRole(Role.Admin));
        }
    }
}