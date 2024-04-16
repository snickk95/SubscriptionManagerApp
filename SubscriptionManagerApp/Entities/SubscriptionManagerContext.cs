using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SubscriptionManagerApp.Entities;

public partial class SubscriptionManagerContext : DbContext
{
    public SubscriptionManagerContext()
    {
    }

    public SubscriptionManagerContext(DbContextOptions<SubscriptionManagerContext> options)
        : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>().HasKey(u => new { u.SubscriptionId, u.UserId });

        modelBuilder.Entity<UserSubscription>().HasOne(s => s.Subscription).WithMany(u => u.UserSubscriptions).HasForeignKey(u => u.SubscriptionId);

        modelBuilder.Entity<UserSubscription>().HasOne(u => u.User).WithMany(u => u.UserSubscriptions).HasForeignKey(u => u.UserId);

        modelBuilder.Entity<User>().HasData(
            new User() { UserId = 1, FirstName = "Sherlock", LastName = "Holmes", Email = "sherlock@example.com" },
            new User() { UserId = 2, FirstName = "John", LastName = "Watson", Email = "john@example.com" }
        );

        modelBuilder.Entity<Subscription>().HasData(
            new Subscription() { SubscriptionId = 1, ServiceName = "Disney Plus", Price = (decimal?)14.99 },
            new Subscription() { SubscriptionId = 2, ServiceName = "Spotify", Price = (decimal?)9.99 },
            new Subscription() { SubscriptionId = 3, ServiceName = "Netflix", Price = (decimal?)18.99 }
        );

        modelBuilder.Entity<UserSubscription>().HasData(
            new UserSubscription() { SubscriptionId = 1, UserId = 1 },
            new UserSubscription() { SubscriptionId = 2, UserId = 1 },
            new UserSubscription() { SubscriptionId = 3, UserId = 1 },
            new UserSubscription() { SubscriptionId = 2, UserId = 2 },
            new UserSubscription() { SubscriptionId = 3, UserId = 2 }
        );

    }
}
