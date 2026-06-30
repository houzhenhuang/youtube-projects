using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using CalConnect.Api.Roles.Domain;

namespace CalConnect.Api.Roles;

internal sealed class UpdateRole(ApplicationDbContext context)
{
    public sealed record Request(int Id, string Name);

    public async Task<bool> Handle(Request request, CancellationToken cancellationToken)
    {
        Role? role = await context.Roles.FindAsync([request.Id], cancellationToken);

        if (role is null)
        {
            return false;
        }

        role.Name = request.Name;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    internal sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPut("api/roles/{id}", async (UpdateRole useCase, Request request, CancellationToken cancellationToken) =>
            {
                var isSucess = await useCase.Handle(request, cancellationToken);

                return isSucess ? Results.NotFound() : Results.BadRequest();
            })
            .WithTags(RoleEndpoint.Tag)
            .RequireAuthorization(policy => policy.RequireRole(Role.Admin)); ;
        }
    }
}
