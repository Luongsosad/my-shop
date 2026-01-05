using HotChocolate;
using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.API.GraphQL;

public class Mutation
{
    // Product Mutations
    public async Task<Product> CreateProduct(
        CreateProductDto input,
        [Service] IProductService productService)
    {
        var product = new Product
        {
            Sku = input.Sku,
            Name = input.Name,
            ImportPrice = input.ImportPrice,
            Count = input.Count,
            Description = input.Description,
            CategoryId = input.CategoryId,
            Images = input.Images,
            IsDeleted = false
        };

        return await productService.CreateProductAsync(product);
    }

    public async Task<Product> UpdateProduct(
        UpdateProductDto input,
        [Service] IProductService productService)
    {
        var product = new Product
        {
            Id = input.Id,
            Sku = input.Sku,
            Name = input.Name,
            ImportPrice = input.ImportPrice,
            Count = input.Count,
            Description = input.Description,
            CategoryId = input.CategoryId,
            Images = input.Images,
            IsDeleted = false
        };

        return await productService.UpdateProductAsync(product);
    }

    public async Task<bool> DeleteProduct(
        int id,
        [Service] IProductService productService)
    {
        return await productService.DeleteProductAsync(id);
    }

    // Customer Mutations
    public async Task<Customer> CreateCustomer(
        string name,
        string phone,
        [Service] IApplicationDbContext context)
    {
        var customer = new Customer
        {
            Name = name,
            Phone = phone,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateCustomer(
        int id,
        string name,
        string phone,
        [Service] IApplicationDbContext context)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer == null)
            throw new GraphQLException("Customer not found");

        customer.Name = name;
        customer.Phone = phone;

        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> DeleteCustomer(
        int id,
        [Service] IApplicationDbContext context)
    {
        var customer = await context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        customer.IsDeleted = true;
        await context.SaveChangesAsync();
        return true;
    }

    // Order Mutations
    public async Task<Order> CreateOrder(
        int? customerId,
        string? customerName,
        string? customerPhone,
        List<OrderItemInput> items,
        int finalPrice,
        [Service] IApplicationDbContext context)
    {
        // If customer details provided but no customerId, create new customer
        int? finalCustomerId = customerId;
        
        if (customerId == null && !string.IsNullOrWhiteSpace(customerName) && !string.IsNullOrWhiteSpace(customerPhone))
        {
            var newCustomer = new Customer
            {
                Name = customerName,
                Phone = customerPhone,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };
            
            context.Customers.Add(newCustomer);
            await context.SaveChangesAsync();
            
            finalCustomerId = newCustomer.Id;
        }
        
        var order = new Order
        {
            CustomerId = finalCustomerId,
            CreatedTime = DateTime.UtcNow,
            FinalPrice = finalPrice,
            OrderItems = items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // Update product stock
        foreach (var item in items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Count -= item.Quantity;
            }
        }
        await context.SaveChangesAsync();

        return order;
    }

    // Category Mutations
    public async Task<Category> CreateCategory(
        string name,
        string? description,
        [Service] IApplicationDbContext context)
    {
        var category = new Category
        {
            Name = name,
            Description = description
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategory(
        int id,
        string name,
        string? description,
        [Service] IApplicationDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
            throw new GraphQLException("Category not found");

        category.Name = name;
        category.Description = description;

        await context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategory(
        int id,
        [Service] IApplicationDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null)
            return false;

        // Check if category has products
        var hasProducts = await context.Products.AnyAsync(p => p.CategoryId == id && !p.IsDeleted);
        if (hasProducts)
            throw new GraphQLException("Cannot delete category with existing products");

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    // Admin Mutations
    public async Task<Admin> AdminSignup(
        string email,
        string password,
        string name,
        [Service] IAdminService adminService)
    {
        return await adminService.CreateAsync(email, password, name);
    }

    public async Task<AdminLoginResponse> AdminLogin(
        string email,
        string password,
        [Service] IAdminService adminService)
    {
        var token = await adminService.SessionAsync(email, password);
        return new AdminLoginResponse
        {
            Email = email,
            Token = token
        };
    }
}

// Input types
public record OrderItemInput(int ProductId, int Quantity, int Price);

public class AdminLoginResponse
{
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}

