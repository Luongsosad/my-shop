using Microsoft.EntityFrameworkCore;
using my_shop.Infrastructure.Db;
using my_shop.Domain.Entities;
using HotChocolate;
using HotChocolate.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyShopDbContext>(options =>
    options.UseSqlite("Data Source=myshop.db"));

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyShopDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.Categories.Any())
    {
        db.Categories.Add(new Category { Name = "Electronics", Description = "Gadgets and devices" });
        db.Categories.Add(new Category { Name = "Clothing", Description = "Apparel and fashion" });
        db.SaveChanges();
    }

    if (!db.Products.Any())
    {
        var catElec = db.Categories.First(c => c.Name == "Electronics");
        db.Products.Add(new Product 
        { 
            Name = "Smartphone X", 
            Sku = "SPX-001", 
            ImportPrice = 500, 
            Count = 10, 
            CategoryId = catElec.Id,
            ExtensionData = "{\"Screen\": \"OLED\", \"Battery\": \"5000mAh\"}"
        });
        db.SaveChanges();
    }
}

app.MapGraphQL();

app.Run();

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] MyShopDbContext context) =>
        context.Products;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] MyShopDbContext context) =>
        context.Categories;
}
