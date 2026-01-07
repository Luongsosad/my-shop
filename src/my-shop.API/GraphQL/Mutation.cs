using Microsoft.EntityFrameworkCore;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.API.GraphQL;

public class Mutation
{
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
    public async Task<Order> UpdateOrderStatus(
    int id,
    string status,
    [Service] IApplicationDbContext context)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
            throw new Exception("Order not found");

        order.Status = status;
        await context.SaveChangesAsync();

        return order;
    }
    public async Task<Order> UpdateOrder(
        int id,
        List<OrderItemInput> items,
        [Service] IApplicationDbContext context)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new Exception("Order not found");

        context.OrderItems.RemoveRange(order.OrderItems);

        var newOrderItems = items.Select(input => new OrderItem
        {
            OrderId = id,
            ProductId = input.ProductId,
            Quantity = input.Quantity,
            Price = input.Price
        }).ToList();

        context.OrderItems.AddRange(newOrderItems);

        order.FinalPrice = newOrderItems.Sum(item => item.Quantity * item.Price);

        await context.SaveChangesAsync();

        return await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id) ?? order;
    }
}

public class AdminLoginResponse
{
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}

public class OrderItemInput
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
}