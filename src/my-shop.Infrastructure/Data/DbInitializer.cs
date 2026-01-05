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
        try
        {
            // Check if tables exist by trying to query them
            var tableExists = await context.Database.CanConnectAsync();
            if (!tableExists)
            {
                Console.WriteLine("??  Database tables not ready. Skipping seeding.");
                return;
            }

            // Seed Admins
            try
            {
                if (!await context.Admins.AnyAsync())
                {
                    Console.WriteLine("?? Seeding admins...");
                    var admins = new List<Admin>
                    {
                        new Admin 
                        { 
                            Email = "admin@myshop.com", 
                            Password = "Admin@123", // In production, this should be hashed
                            Name = "Super Admin",
                            IsInvoke = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Admin 
                        { 
                            Email = "manager@myshop.com", 
                            Password = "Manager@123",
                            Name = "Shop Manager",
                            IsInvoke = false,
                            CreatedAt = DateTime.UtcNow
                        }
                    };
                    await context.Admins.AddRangeAsync(admins);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"? Seeded {admins.Count} admins successfully.");
                }
                else
                {
                    Console.WriteLine($"??  Admins already exist: {await context.Admins.CountAsync()} admins found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"??  Warning: Could not seed admins. Table may not exist yet. {ex.Message}");
            }

            // Seed Customers
            try
            {
                if (!await context.Customers.AnyAsync())
                {
                    Console.WriteLine("?? Seeding customers...");
                    var customers = new List<Customer>
                    {
                        new Customer { Name = "Nguy?n V?n A", Phone = "0901234567", IsDeleted = false },
                        new Customer { Name = "Tr?n Th? B", Phone = "0912345678", IsDeleted = false },
                        new Customer { Name = "Lê V?n C", Phone = "0923456789", IsDeleted = false },
                        new Customer { Name = "Ph?m Th? D", Phone = "0934567890", IsDeleted = false },
                        new Customer { Name = "Hoàng V?n E", Phone = "0945678901", IsDeleted = false },
                        new Customer { Name = "V? Th? F", Phone = "0956789012", IsDeleted = false },
                        new Customer { Name = "??ng V?n G", Phone = "0967890123", IsDeleted = false },
                        new Customer { Name = "Bùi Th? H", Phone = "0978901234", IsDeleted = false },
                        new Customer { Name = "Võ V?n I", Phone = "0989012345", IsDeleted = false },
                        new Customer { Name = "?? Th? K", Phone = "0990123456", IsDeleted = false }
                    };
                    await context.Customers.AddRangeAsync(customers);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"? Seeded {customers.Count} customers successfully.");
                }
                else
                {
                    Console.WriteLine($"??  Customers already exist: {await context.Customers.CountAsync()} customers found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"??  Warning: Could not seed customers. Table may not exist yet. {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"??  Warning: Could not complete seeding. {ex.Message}");
        }

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

        // Seed Products with images and IsDeleted
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
                // Electronics
                new Product 
                { 
                    Name = "iPhone 15 Pro", 
                    Sku = "IPH-15P", 
                    ImportPrice = 25000000, 
                    Count = 3, 
                    CategoryId = electronics.Id, 
                    Description = "Latest iPhone model with A17 Pro chip",
                    IsDeleted = false,
                    Images = new List<string> 
                    { 
                        "https://res.cloudinary.com/demo/image/upload/v1/products/iphone-15-pro.jpg" 
                    }
                },
                new Product 
                { 
                    Name = "Samsung Galaxy S24", 
                    Sku = "SGS-24", 
                    ImportPrice = 22000000, 
                    Count = 2, 
                    CategoryId = electronics.Id, 
                    Description = "Flagship Samsung phone with AI features",
                    IsDeleted = false,
                    Images = new List<string> 
                    { 
                        "https://res.cloudinary.com/demo/image/upload/v1/products/galaxy-s24.jpg" 
                    }
                },
                new Product 
                { 
                    Name = "MacBook Pro M3", 
                    Sku = "MBP-M3", 
                    ImportPrice = 45000000, 
                    Count = 15, 
                    CategoryId = electronics.Id, 
                    Description = "Professional laptop with M3 chip",
                    IsDeleted = false,
                    Images = new List<string> 
                    { 
                        "https://res.cloudinary.com/demo/image/upload/v1/products/macbook-pro-m3.jpg" 
                    }
                },
                new Product 
                { 
                    Name = "AirPods Pro", 
                    Sku = "APP-2", 
                    ImportPrice = 5500000, 
                    Count = 4, 
                    CategoryId = electronics.Id, 
                    Description = "Wireless earbuds with noise cancellation",
                    IsDeleted = false,
                    Images = new List<string> 
                    { 
                        "https://res.cloudinary.com/demo/image/upload/v1/products/airpods-pro.jpg" 
                    }
                },
                new Product 
                { 
                    Name = "iPad Air", 
                    Sku = "IPA-AIR", 
                    ImportPrice = 15000000, 
                    Count = 8, 
                    CategoryId = electronics.Id, 
                    Description = "Versatile tablet for work and play",
                    IsDeleted = false,
                    Images = new List<string> 
                    { 
                        "https://res.cloudinary.com/demo/image/upload/v1/products/ipad-air.jpg" 
                    }
                },
                
                // Clothing
                new Product 
                { 
                    Name = "Nike Air Max", 
                    Sku = "NKE-AM", 
                    ImportPrice = 3200000, 
                    Count = 25, 
                    CategoryId = clothing.Id, 
                    Description = "Running shoes with air cushioning",
                    IsDeleted = false
                },
                new Product 
                { 
                    Name = "Levi's 501 Jeans", 
                    Sku = "LVS-501", 
                    ImportPrice = 1500000, 
                    Count = 1, 
                    CategoryId = clothing.Id, 
                    Description = "Classic straight fit jeans",
                    IsDeleted = false
                },
                new Product 
                { 
                    Name = "Adidas T-Shirt", 
                    Sku = "ADS-TSH", 
                    ImportPrice = 450000, 
                    Count = 50, 
                    CategoryId = clothing.Id, 
                    Description = "Comfortable sports t-shirt",
                    IsDeleted = false
                },
                
                // Books
                new Product 
                { 
                    Name = "Clean Code", 
                    Sku = "BK-CC", 
                    ImportPrice = 320000, 
                    Count = 30, 
                    CategoryId = books.Id, 
                    Description = "A handbook of agile software craftsmanship",
                    IsDeleted = false
                },
                new Product 
                { 
                    Name = "Atomic Habits", 
                    Sku = "BK-AH", 
                    ImportPrice = 280000, 
                    Count = 20, 
                    CategoryId = books.Id, 
                    Description = "Tiny changes, remarkable results",
                    IsDeleted = false
                },
                
                // Home & Kitchen
                new Product 
                { 
                    Name = "Dyson Vacuum", 
                    Sku = "DYS-VC", 
                    ImportPrice = 12000000, 
                    Count = 7, 
                    CategoryId = homeKitchen.Id, 
                    Description = "Cordless vacuum cleaner V15",
                    IsDeleted = false
                },
                new Product 
                { 
                    Name = "Air Fryer", 
                    Sku = "AF-001", 
                    ImportPrice = 2500000, 
                    Count = 12, 
                    CategoryId = homeKitchen.Id, 
                    Description = "Digital air fryer 5.5L capacity",
                    IsDeleted = false
                },
                
                // Sports
                new Product 
                { 
                    Name = "Yoga Mat", 
                    Sku = "YG-MT", 
                    ImportPrice = 350000, 
                    Count = 45, 
                    CategoryId = sports.Id, 
                    Description = "Non-slip yoga mat with carrying strap",
                    IsDeleted = false
                },
                new Product 
                { 
                    Name = "Dumbbells 10kg", 
                    Sku = "DB-10K", 
                    ImportPrice = 800000, 
                    Count = 18, 
                    CategoryId = sports.Id, 
                    Description = "Set of 2 dumbbells 10kg each",
                    IsDeleted = false
                },
                
                // Deleted product example
                new Product 
                { 
                    Name = "Old iPhone 11", 
                    Sku = "IPH-11-DEL", 
                    ImportPrice = 12000000, 
                    Count = 0, 
                    CategoryId = electronics.Id, 
                    Description = "Discontinued product",
                    IsDeleted = true
                }
            };
            
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"? Seeded {products.Count} products (including {products.Count(p => p.IsDeleted)} deleted) successfully.");
        }
        else
        {
            Console.WriteLine($"??  Products already exist: {await context.Products.CountAsync()} products found.");
        }

        // Seed Orders with Customer references
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
        var products = await context.Products.Where(p => !p.IsDeleted).OrderBy(p => p.Id).ToListAsync();
        var customers = await context.Customers.Where(c => !c.IsDeleted).ToListAsync();
        
        if (products.Count < 10 || customers.Count < 3)
        {
            Console.WriteLine($"??  Warning: Not enough data. Need at least 10 products and 3 customers. Skipping order seeding.");
            return;
        }

        Console.WriteLine("?? Seeding orders...");
        var today = DateTime.UtcNow.Date;
        var random = new Random();
        
        var orders = new List<Order>();
        
        // Today's orders with customers
        orders.Add(new Order 
        { 
            CustomerId = customers[0].Id,
            CreatedTime = today.AddHours(9), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[0].Id, Quantity = 1, Price = 27000000 },
                new OrderItem { ProductId = products[3].Id, Quantity = 2, Price = 6000000 }
            }
        });
        
        orders.Add(new Order 
        { 
            CustomerId = customers[1].Id,
            CreatedTime = today.AddHours(11), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[2].Id, Quantity = 1, Price = 48000000 }
            }
        });
        
        orders.Add(new Order 
        { 
            CustomerId = customers[2].Id,
            CreatedTime = today.AddHours(14), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[5].Id, Quantity = 3, Price = 3500000 },
                new OrderItem { ProductId = products[7].Id, Quantity = 5, Price = 500000 }
            }
        });
        
        // Yesterday's orders
        orders.Add(new Order 
        { 
            CustomerId = customers[random.Next(customers.Count)].Id,
            CreatedTime = today.AddDays(-1).AddHours(10), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[1].Id, Quantity = 2, Price = 24000000 }
            }
        });
        
        orders.Add(new Order 
        { 
            CustomerId = customers[random.Next(customers.Count)].Id,
            CreatedTime = today.AddDays(-1).AddHours(15), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[10].Id, Quantity = 1, Price = 13000000 }
            }
        });
        
        // This month's orders
        for (int i = 2; i <= 15; i++)
        {
            var customer = customers[random.Next(customers.Count)];
            var productCount = random.Next(1, 4);
            var orderItems = new List<OrderItem>();
            
            for (int j = 0; j < productCount; j++)
            {
                var product = products[random.Next(products.Count)];
                orderItems.Add(new OrderItem 
                { 
                    ProductId = product.Id, 
                    Quantity = random.Next(1, 5), 
                    Price = product.ImportPrice + random.Next(-500000, 500000)
                });
            }
            
            orders.Add(new Order 
            { 
                CustomerId = customer.Id,
                CreatedTime = today.AddDays(-i).AddHours(random.Next(9, 18)), 
                FinalPrice = 0,
                OrderItems = orderItems
            });
        }
        
        // Some orders without customer (walk-in customers)
        orders.Add(new Order 
        { 
            CustomerId = null,
            CreatedTime = today.AddDays(-3).AddHours(13), 
            FinalPrice = 0,
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = products[12].Id, Quantity = 2, Price = 400000 }
            }
        });
        
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
            Console.WriteLine($"   - {orders.Count(o => o.CustomerId != null)} orders with customer");
            Console.WriteLine($"   - {orders.Count(o => o.CustomerId == null)} orders without customer (walk-in)");
        }
    }
}
