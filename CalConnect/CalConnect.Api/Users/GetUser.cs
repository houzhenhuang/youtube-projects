using CalConnect.Api.Database;
using CalConnect.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class GetUser(ApplicationDbContext context)
{
    public sealed record UserResponse(Guid Id, string FirstName, string LastName, string Email, bool EmailVerified);

    public async Task<UserResponse?> Handle(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? null : new UserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.EmailVerified);
    }
}

public class GetUserEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users/{id:guid}", async (Guid id, GetUser useCase) =>
            {
                GetUser.UserResponse? user = await useCase.Handle(id);
                return user is not null ? Results.Ok(user) : Results.NotFound();
            })
            .WithTags(UserEndpoints.Tag)
            .RequireAuthorization();
    }
}