using CalConnect.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Users;

internal sealed class GetUser(ApplicationDbContext context)
{
    public sealed record UserResponse(Guid Id, string Email, string FirstName, string LastName);

    public async Task<UserResponse?> Handle(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? null : new UserResponse(user.Id, user.Email, user.FirstName, user.LastName);
    }
}