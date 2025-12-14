using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using my_shop.Domain.Entities;
using my_shop.Infrastructure.Db;

namespace my_shop.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();

        try
        {
            // Apply pending migrations
            Console.WriteLine("?? Checking for pending database migrations...");
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"?? Found {pendingMigrations.Count()} pending migration(s):");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"   - {migration}");
                }
                
                Console.WriteLine("??  Applying migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("? Migrations applied successfully!");
            }
            else
            {
                Console.WriteLine("? Database is up to date. No pending migrations.");
            }
            
            Console.WriteLine($"? Database connection: {(context.Database.CanConnect() ? "SUCCESS" : "FAILED")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? ERROR: Cannot connect to database or apply migrations.");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("\nTroubleshooting:");
            Console.WriteLine("1. Make sure database 'My-Shop' exists on Neon.tech");
            Console.WriteLine("2. Check your .env file has correct DATABASE_URL");
            Console.WriteLine("3. Verify network connectivity to database");
            throw;
        }

        await SeedDataAsync(context);
    }

    private static async Task SeedDataAsync(MyShopDbContext context)
    {
        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            Console.WriteLine("?? Seeding categories...");
            var categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { Name = "Clothing", Description = "Fashion and apparel" },
                new Category { Name = "Books", Description = "Books and magazines" },
                new Category { Name = "Home & Kitchen", Description = "Home appliances and kitchenware" },
                new Category { Name = "Sports", Description = "Sports equipment and accessories" }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"? Seeded {categories.Count} categories successfully.");
        }
        else
        {
            Console.WriteLine($"??  Categories already exist: {await context.Categories.CountAsync()} categories found.");
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            Console.WriteLine("?? Seeding products...");
            var electronics = await context.Categories.FirstAsync(c => c.Name == "Electronics");
            var clothing = await context.Categories.FirstAsync(c => c.Name == "Clothing");
            var books = await context.Categories.FirstAsync(c => c.Name == "Books");
            var homeKitchen = await context.Categories.FirstAsync(c => c.Name == "Home & Kitchen");
            var sports = await context.Categories.FirstAsync(c => c.Name == "Sports");

            var products = new List<Product>
            {
                // Electronics - Some low stock
                new Product { Name = "iPhone 15 Pro", Sku = "IPH-15P", ImportPrice = 25000000, Count = 3, CategoryId = electronics.Id, Description = "Latest iPhone model" },
                new Product { Name = "Samsung Galaxy S24", Sku = "SGS-24", ImportPrice = 22000000, Count = 2, CategoryId = electronics.Id, Description = "Flagship Samsung phone" },
                new Product { Name = "MacBook Pro M3", Sku = "MBP-M3", ImportPrice = 45000000, Count = 15, CategoryId = electronics.Id, Description = "Professional laptop" },
                new Product { Name = "AirPods Pro", Sku = "APP-2", ImportPrice = 5500000, Count = 4, CategoryId = electronics.Id, Description = "Wireless earbuds" },
                new Product { Name = "iPad Air", Sku = "IPA-AIR", ImportPrice = 15000000, Count = 8, CategoryId = electronics.Id, Description = "Versatile tablet" },
                
                // Clothing
                new Product { Name = "Nike Air Max", Sku = "NKE-AM", ImportPrice = 3200000, Count = 25, CategoryId = clothing.Id, Description = "Running shoes" },
                new Product { Name = "Levi's 501 Jeans", Sku = "LVS-501", ImportPrice = 1500000, Count = 1, CategoryId = clothing.Id, Description = "Classic jeans" },
                new Product { Name = "Adidas T-Shirt", Sku = "ADS-TSH", ImportPrice = 450000, Count = 50, CategoryId = clothing.Id, Description = "Sports t-shirt" },
                
                // Books
                new Product { Name = "Clean Code", Sku = "BK-CC", ImportPrice = 320000, Count = 30, CategoryId = books.Id, Description = "Programming book" },
                new Product { Name = "Atomic Habits", Sku = "BK-AH", ImportPrice = 280000, Count = 20, CategoryId = books.Id, Description = "Self-help book" },
                
                // Home & Kitchen
                new Product { Name = "Dyson Vacuum", Sku = "DYS-VC", ImportPrice = 12000000, Count = 7, CategoryId = homeKitchen.Id, Description = "Cordless vacuum" },
                new Product { Name = "Air Fryer", Sku = "AF-001", ImportPrice = 2500000, Count = 12, CategoryId = homeKitchen.Id, Description = "Digital air fryer" },
                
                // Sports
                new Product { Name = "Yoga Mat", Sku = "YG-MT", ImportPrice = 350000, Count = 45, CategoryId = sports.Id, Description = "Non-slip yoga mat" },
                new Product { Name = "Dumbbells 10kg", Sku = "DB-10K", ImportPrice = 800000, Count = 18, CategoryId = sports.Id, Description = "Set of dumbbells" }
            };
            
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"? Seeded {products.Count} products successfully.");
        }
        else
        {
            Console.WriteLine($"??  Products already exist: {await context.Products.CountAsync()} products found.");
        }

        // Seed Orders with items (for dashboard statistics)
        if (!await context.Orders.AnyAsync())
        {
            await SeedOrdersAsync(context);
        }
        else
        {
            Console.WriteLine($"??  Orders already exist: {await context.Orders.CountAsync()} orders found.");
        }
    }

    private static async Task SeedOrdersAsync(MyShopDbContext context)
    {
        var products = await context.Products.OrderBy(p => p.Id).ToListAsync();
        
        if (products.Count < 14)
        {
            Console.WriteLine($"??  Warning: Only {products.Count} products available. Skipping order seeding.");
            return;
        }

        Console.WriteLine("?? Seeding orders...");
        var today = DateTime.UtcNow.Date;
        
        // Find products by SKU
        var iphone = products.FirstOrDefault(p => p.Sku == "IPH-15P");
        var samsung = products.FirstOrDefault(p => p.Sku == "SGS-24");
        var macbook = products.FirstOrDefault(p => p.Sku == "MBP-M3");
        var airpods = products.FirstOrDefault(p => p.Sku == "APP-2");
        var ipad = products.FirstOrDefault(p => p.Sku == "IPA-AIR");
        var nike = products.FirstOrDefault(p => p.Sku == "NKE-AM");
        var jeans = products.FirstOrDefault(p => p.Sku == "LVS-501");
        var tshirt = products.FirstOrDefault(p => p.Sku == "ADS-TSH");
        var cleanCode = products.FirstOrDefault(p => p.Sku == "BK-CC");
        var dyson = products.FirstOrDefault(p => p.Sku == "DYS-VC");
        var airFryer = products.FirstOrDefault(p => p.Sku == "AF-001");
        
        var orders = new List<Order>();
        
        // Today's orders
        if (iphone != null && airpods != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddHours(9), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = iphone.Id, Quantity = 1, Price = 27000000 },
                    new OrderItem { ProductId = airpods.Id, Quantity = 2, Price = 6000000 }
                }
            });
        }
        
        if (macbook != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddHours(11), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = macbook.Id, Quantity = 1, Price = 48000000 }
                }
            });
        }
        
        if (nike != null && tshirt != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddHours(14), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = nike.Id, Quantity = 3, Price = 3500000 },
                    new OrderItem { ProductId = tshirt.Id, Quantity = 5, Price = 500000 }
                }
            });
        }
        
        // Yesterday's orders
        if (samsung != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddDays(-1).AddHours(10), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = samsung.Id, Quantity = 2, Price = 24000000 }
                }
            });
        }
        
        if (dyson != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddDays(-1).AddHours(15), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = dyson.Id, Quantity = 1, Price = 13000000 }
                }
            });
        }
        
        // This month's orders (past days)
        if (ipad != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddDays(-5), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = ipad.Id, Quantity = 2, Price = 16000000 }
                }
            });
        }
        
        if (cleanCode != null && airFryer != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddDays(-10), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = cleanCode.Id, Quantity = 10, Price = 350000 },
                    new OrderItem { ProductId = airFryer.Id, Quantity = 3, Price = 2700000 }
                }
            });
        }
        
        if (jeans != null)
        {
            orders.Add(new Order 
            { 
                CreatedTime = today.AddDays(-15), 
                FinalPrice = 0,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = jeans.Id, Quantity = 4, Price = 1600000 }
                }
            });
        }
        
        // Calculate final prices
        foreach (var order in orders)
        {
            order.FinalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
        }
        
        if (orders.Any())
        {
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
            Console.WriteLine($"? Seeded {orders.Count} orders successfully.");
        }
    }
}
