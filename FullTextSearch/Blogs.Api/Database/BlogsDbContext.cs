using Microsoft.EntityFrameworkCore;

namespace Blogs.Api.Database;

public class BlogsDbContext : DbContext
{
    public BlogsDbContext(DbContextOptions<BlogsDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogsDbContext).Assembly);
    }
}