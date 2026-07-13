using Microsoft.EntityFrameworkCore;
using Notes.Api.Domain;

namespace Notes.Api.Data;

public static class QueryableExtensions
{
    public static IQueryable<T> ForTenant<T>(this DbSet<T> dbSet, Guid tenantId)
        where T : class, ITenantOwned
    {
        return dbSet.Where(e => e.TenantId == tenantId);
    }
}
