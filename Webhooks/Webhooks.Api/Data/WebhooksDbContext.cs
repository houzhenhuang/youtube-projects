using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Models;

namespace Webhooks.Api.Data;

public class WebhooksDbContext(DbContextOptions<WebhooksDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
    public DbSet<WebhookDeliveryAttempt> WebhookDeliveryAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.ToTable("orders");
            builder.HasKey(o => o.Id);
        });

        modelBuilder.Entity<WebhookSubscription>(builder =>
        {
            builder.ToTable("subscriptions", "webhooks");
            builder.HasKey(o => o.Id);
        });
        modelBuilder.Entity<WebhookDeliveryAttempt>(builder =>
        {
            builder.ToTable("delivery_attempts", "webhooks");
            builder.HasKey(o => o.Id);

            builder.HasOne<WebhookSubscription>()
                .WithMany()
                .HasForeignKey(d => d.WebhookSubscriptionId);
        });
    }
}