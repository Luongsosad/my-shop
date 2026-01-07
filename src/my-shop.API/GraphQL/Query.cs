using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.API.GraphQL;

public class Query
{
    // Product Queries
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Product>> GetProducts([Service] IProductService productService)
    {
        return await productService.GetProductsAsync();
    }

    // Category Queries
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Category> GetCategories([Service] IApplicationDbContext context)
    {
        return context.Categories;
    }

    // Order Queries
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Order> GetOrders([Service] IApplicationDbContext context)
    {
        return context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] IApplicationDbContext context)
    {
        return context.Customers.Where(c => !c.IsDeleted);
    }

    // Dashboard Statistics
    public async Task<DashboardStatsDto> GetDashboardStats([Service] IDashboardService dashboardService)
    {
        return await dashboardService.GetDashboardStatsAsync();
    }

    // Recent Orders
    public async Task<List<RecentOrderDto>> GetRecentOrders(
        [Service] IDashboardService dashboardService, 
        int count = 3)
    {
        return await dashboardService.GetRecentOrdersAsync(count);
    }

    // Low Stock Products
    public async Task<List<LowStockProductDto>> GetLowStockProducts(
        [Service] IDashboardService dashboardService, 
        int threshold = 5, 
        int count = 5)
    {
        return await dashboardService.GetLowStockProductsAsync(threshold, count);
    }

    // Best Selling Products
    public async Task<List<BestSellingProductDto>> GetBestSellingProducts(
        [Service] IDashboardService dashboardService, 
        int count = 5)
    {
        return await dashboardService.GetBestSellingProductsAsync(count);
    }


}
