using CustomerManager.Api.Application.Abstracts.Authentication;
using CustomerManager.Api.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using OpenApiSamples.Data;

namespace CustomerManager.Api.Infrastructure.Authentication;

public sealed class PermissionService : IPermissionService
{
    private readonly AppDbContext _dbContext;

    public PermissionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<HashSet<string>> GetPermissionsAsync(int userId)
    {
        ICollection<Role>[] roles = await _dbContext.Set<User>()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .Where(x => x.Id == userId)
            .Select(x => x.Roles)
            .ToArrayAsync();

        return roles
            .SelectMany(x => x)
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
