using Microsoft.EntityFrameworkCore;
using Notes.Api.Domain;
using Notes.Api.Services;

namespace Notes.Api.Data;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.TenantId).IsUnique();
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.TenantId);
            entity.HasIndex(x => x.TenantId);
        });
    }

    //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //{
    //    foreach (var entry in ChangeTracker.Entries<ITenantOwned>())
    //    {
    //        if (entry.State == EntityState.Added)
    //        {
    //            entry.Property(e => e.TenantId).CurrentValue = await userContext.GetTenantId();
    //        }
    //    }

    //    return await base.SaveChangesAsync(cancellationToken);
    //}
}
