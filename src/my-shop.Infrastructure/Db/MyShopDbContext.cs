using Microsoft.EntityFrameworkCore;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.Infrastructure.Db;

public class MyShopDbContext : DbContext, IApplicationDbContext
{
    public MyShopDbContext(DbContextOptions<MyShopDbContext> options) : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Admin configurations
        modelBuilder.Entity<Admin>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<Admin>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Admin>()
            .Property(a => a.IsInvoke)
            .HasDefaultValue(false);

        // Session configurations
        modelBuilder.Entity<Session>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Session>()
            .Property(s => s.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.Admin)
            .WithMany(a => a.Sessions)
            .HasForeignKey(s => s.AdminId)
            .OnDelete(DeleteBehavior.Cascade);

        // Customer configurations
        modelBuilder.Entity<Customer>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Customer>()
            .Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Phone);

        // Product configurations
        modelBuilder.Entity<Product>()
            .Property(p => p.ImportPrice)
            .HasDefaultValue(0);
            
        modelBuilder.Entity<Product>()
            .Property(p => p.Count)
            .HasDefaultValue(0);

        modelBuilder.Entity<Product>()
            .Property(p => p.IsDeleted)
            .HasDefaultValue(false);
            
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Order configurations
        modelBuilder.Entity<Order>()
            .Property(o => o.CreatedTime)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Order>()
            .Property(o => o.FinalPrice)
            .HasDefaultValue(0);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderItem configurations
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
    }
}
