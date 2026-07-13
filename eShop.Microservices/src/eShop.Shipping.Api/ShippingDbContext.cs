using eShop.Shipping.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Shipping.Api;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> options)
    : DbContext(options)
{
    public DbSet<ShipmentRecord> ShipmentRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shipping");

        base.OnModelCreating(modelBuilder);
    }
}

