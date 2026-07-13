using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Notes.Api.Data;

namespace Notes.Api.Services;

public class UserContext(
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext dbContext,
    HybridCache cache) : IUserContext
{
    private const string UserIdClaim = "user_id";

    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(UserIdClaim)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new InvalidOperationException("在token中获取到无效的 UserId");
            }

            return userId;
        }
    }

    public async Task<Guid> GetTenantId()
    {
        var cacheKey = $"user_tenant_{UserId}";

        return await cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var user = await dbContext.Users
            .Where(u => u.Id == UserId)
            .Select(u => new { u.TenantId })
            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("用户不存在");
            }

            return user.TenantId;
        }, new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5)
        });
    }
}
