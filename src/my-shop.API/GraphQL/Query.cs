using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;
using my_shop.Domain.Entities;

namespace my_shop.API.GraphQL;

public class Query
{
    // Product Queries - Filter out deleted products
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] IApplicationDbContext context)
    {
        return context.Products.Where(p => !p.IsDeleted);
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

    // Admin Queries
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Admin> GetAdmins([Service] IApplicationDbContext context)
    {
        return context.Admins;
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

    // Report Queries
    public async Task<List<ProductSalesReportDto>> GetProductSalesReport(
        [Service] IReportService reportService,
        DateTime startDate,
        DateTime endDate,
        ReportPeriodType periodType = ReportPeriodType.Daily,
        int topProducts = 10)
    {
        try
        {
            Console.WriteLine($"[GetProductSalesReport] StartDate: {startDate:yyyy-MM-dd HH:mm:ss}, EndDate: {endDate:yyyy-MM-dd HH:mm:ss}, PeriodType: {periodType}");
            
            var filter = new ReportFilterDto
            {
                StartDate = startDate,
                EndDate = endDate,
                PeriodType = periodType
            };
            var result = await reportService.GetProductSalesReportAsync(filter, topProducts);
            
            Console.WriteLine($"[GetProductSalesReport] Returned {result.Count} products");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetProductSalesReport] ERROR: {ex.Message}");
            Console.WriteLine($"[GetProductSalesReport] Stack: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<RevenueProfitReportDto> GetRevenueProfitReport(
        [Service] IReportService reportService,
        DateTime startDate,
        DateTime endDate,
        ReportPeriodType periodType = ReportPeriodType.Daily)
    {
        try
        {
            Console.WriteLine($"[GetRevenueProfitReport] StartDate: {startDate:yyyy-MM-dd HH:mm:ss}, EndDate: {endDate:yyyy-MM-dd HH:mm:ss}, PeriodType: {periodType}");
            
            var filter = new ReportFilterDto
            {
                StartDate = startDate,
                EndDate = endDate,
                PeriodType = periodType
            };
            var result = await reportService.GetRevenueProfitReportAsync(filter);
            
            Console.WriteLine($"[GetRevenueProfitReport] TotalRevenue: {result.TotalRevenue}, TotalOrders: {result.TotalOrders}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetRevenueProfitReport] ERROR: {ex.Message}");
            Console.WriteLine($"[GetRevenueProfitReport] Stack: {ex.StackTrace}");
            throw;
        }
    }
}
