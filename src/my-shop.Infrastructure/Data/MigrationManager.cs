using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using my_shop.Infrastructure.Db;

namespace my_shop.Infrastructure.Data;

/// <summary>
/// Database migration manager for handling database schema versioning
/// </summary>
public static class MigrationManager
{
    /// <summary>
    /// Get all applied migrations
    /// </summary>
    public static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
        
        return await context.Database.GetAppliedMigrationsAsync();
    }

    /// <summary>
    /// Get all pending migrations
    /// </summary>
    public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
        
        return await context.Database.GetPendingMigrationsAsync();
    }

    /// <summary>
    /// Check if database exists
    /// </summary>
    public static async Task<bool> DatabaseExistsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
        
        return await context.Database.CanConnectAsync();
    }

    /// <summary>
    /// Get current database version (last applied migration)
    /// </summary>
    public static async Task<string?> GetCurrentVersionAsync(IServiceProvider serviceProvider)
    {
        var appliedMigrations = await GetAppliedMigrationsAsync(serviceProvider);
        return appliedMigrations.LastOrDefault();
    }

    /// <summary>
    /// Apply all pending migrations
    /// </summary>
    public static async Task<bool> ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (!pendingMigrations.Any())
            {
                Console.WriteLine("? No pending migrations to apply.");
                return true;
            }

            Console.WriteLine($"?? Applying {pendingMigrations.Count()} migration(s)...");
            foreach (var migration in pendingMigrations)
            {
                Console.WriteLine($"   ? {migration}");
            }

            await context.Database.MigrateAsync();
            
            Console.WriteLine("? All migrations applied successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Failed to apply migrations: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Rollback to a specific migration (requires manual SQL execution)
    /// </summary>
    public static async Task<string> GenerateRollbackScriptAsync(
        IServiceProvider serviceProvider, 
        string targetMigration)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
        
        // Get SQL script to rollback to target migration
        var script = context.Database.GenerateCreateScript();
        
        return $@"
-- Rollback Script
-- Target Migration: {targetMigration}
-- Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}
--
-- WARNING: This will delete data! Make sure you have a backup!
--
-- To rollback manually:
-- 1. Drop all tables
-- 2. Delete records from __EFMigrationsHistory where MigrationId > '{targetMigration}'
-- 3. Run: dotnet ef database update {targetMigration} --startup-project src/my-shop.API

-- Manual SQL for emergency rollback:
DELETE FROM ""__EFMigrationsHistory"" 
WHERE ""MigrationId"" > '{targetMigration}';

-- Note: Tables will be recreated when you run the application next time
";
    }

    /// <summary>
    /// Get migration history report
    /// </summary>
    public static async Task<string> GetMigrationHistoryAsync(IServiceProvider serviceProvider)
    {
        var applied = await GetAppliedMigrationsAsync(serviceProvider);
        var pending = await GetPendingMigrationsAsync(serviceProvider);
        var currentVersion = await GetCurrentVersionAsync(serviceProvider);

        var report = new System.Text.StringBuilder();
        report.AppendLine("?? Database Migration History");
        report.AppendLine("".PadRight(50, '='));
        report.AppendLine($"Current Version: {currentVersion ?? "None"}");
        report.AppendLine($"Applied: {applied.Count()} | Pending: {pending.Count()}");
        report.AppendLine();

        report.AppendLine("? Applied Migrations:");
        foreach (var migration in applied)
        {
            report.AppendLine($"   ? {migration}");
        }

        if (pending.Any())
        {
            report.AppendLine();
            report.AppendLine("? Pending Migrations:");
            foreach (var migration in pending)
            {
                report.AppendLine($"   ? {migration}");
            }
        }

        return report.ToString();
    }

    /// <summary>
    /// Validate database schema integrity
    /// </summary>
    public static async Task<bool> ValidateSchemaAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
            
            // Check if all expected tables exist
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                Console.WriteLine("? Cannot connect to database");
                return false;
            }

            // Check for pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"??  Warning: {pendingMigrations.Count()} pending migration(s) found");
                return false;
            }

            // Try to query each table
            var categoriesCount = await context.Categories.CountAsync();
            var productsCount = await context.Products.CountAsync();
            var ordersCount = await context.Orders.CountAsync();
            var orderItemsCount = await context.OrderItems.CountAsync();

            Console.WriteLine("? Schema validation passed");
            Console.WriteLine($"   Tables: categories({categoriesCount}), products({productsCount}), orders({ordersCount}), order_items({orderItemsCount})");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Schema validation failed: {ex.Message}");
            return false;
        }
    }
}
