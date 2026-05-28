using ContentPlatform.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContentPlatform.Api.Database;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("content");
    }

    public DbSet<Article> Articles { get; set; }
}