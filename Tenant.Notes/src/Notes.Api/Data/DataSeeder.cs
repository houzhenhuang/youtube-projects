using Notes.Api.Domain;

namespace Notes.Api.Data;

public class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var tenant1Id = Guid.NewGuid();
        var tenant2Id = Guid.NewGuid();

        var user1 = new User
        {
            Id = Guid.Parse("7007081e-6f28-4893-a06a-f7b8fa34593f"),
            TenantId = tenant1Id,
            Email = "user@tenant1.com",
        };

        var user2 = new User
        {
            Id = Guid.Parse("d1f8c3e4-5b6a-4c9e-9f7b-2d3e4f5a6b7c"),
            TenantId = tenant2Id,
            Email = "user@tenant2.com",
        };

        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();
    }
}
