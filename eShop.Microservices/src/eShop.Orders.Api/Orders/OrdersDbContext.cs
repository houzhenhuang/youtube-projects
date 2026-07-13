using eShop.Orders.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Orders.Api.Orders;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.HasMany(o => o.Items)
              .WithOne(i => i.Order)
              .HasForeignKey(i => i.OrderId);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(oi => oi.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}

