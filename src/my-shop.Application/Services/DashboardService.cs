using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;

namespace my_shop.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _context;

    public DashboardService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        
        var totalProducts = await _context.Products.CountAsync();
        var todayOrders = await _context.Orders.CountAsync(o => o.CreatedTime.Date == today);
        var todayRevenue = await _context.Orders
            .Where(o => o.CreatedTime.Date == today)
            .SumAsync(o => (long)o.FinalPrice);
        var lowStockCount = await _context.Products.CountAsync(p => p.Count < 5);

        return new DashboardStatsDto
        {
            TotalProducts = totalProducts,
            TodayOrders = todayOrders,
            TodayRevenue = todayRevenue,
            LowStockCount = lowStockCount
        };
    }

    public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 3)
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedTime)
            .Take(count)
            .Select(o => new RecentOrderDto
            {
                OrderId = $"Order #{o.Id}",
                CreatedTime = o.CreatedTime.ToString("dd/MM/yyyy HH:mm"),
                ItemCount = o.OrderItems.Count,
                TotalPrice = o.FinalPrice
            })
            .ToListAsync();
    }

    public async Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 5, int count = 5)
    {
        return await _context.Products
            .Where(p => p.Count < threshold && !p.IsDeleted)
            .OrderBy(p => p.Count)
            .Take(count)
            .Select(p => new LowStockProductDto
            {
                Name = p.Name,
                Sku = p.Sku,
                Count = p.Count,
                Images = p.Images
            })
            .ToListAsync();
    }

    public async Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 5)
    {
        var productSales = await _context.OrderItems
            .Where(oi => !oi.Product.IsDeleted)
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(count)
            .ToListAsync();

        if (!productSales.Any())
            return new List<BestSellingProductDto>();

        var maxSold = productSales.First().TotalSold;

        var result = new List<BestSellingProductDto>();
        foreach (var ps in productSales)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == ps.ProductId);

            result.Add(new BestSellingProductDto
            {
                Name = product.Name,
                Category = product.Category?.Name ?? "Unknown",
                SoldCount = ps.TotalSold,
                SoldPercentage = (double)ps.TotalSold / maxSold * 100,
                Images = product.Images
            });
        }

        return result;
    }

    public async Task<List<DailyRevenueDto>> GetMonthlyRevenueChartAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var ordersData = await _context.Orders
            .Where(o => o.CreatedTime >= startDate && o.CreatedTime <= endDate)
            .GroupBy(o => o.CreatedTime.Day)
            .Select(g => new
            {
                Day = g.Key,
                Revenue = g.Sum(o => (long)o.FinalPrice),
                OrderCount = g.Count()
            })
            .ToListAsync();

        var result = new List<DailyRevenueDto>();
        for (int day = 1; day <= daysInMonth; day++)
        {
            var dayData = ordersData.FirstOrDefault(d => d.Day == day);
            result.Add(new DailyRevenueDto
            {
                Day = day,
                Revenue = dayData?.Revenue ?? 0,
                OrderCount = dayData?.OrderCount ?? 0
            });
        }

        return result;
    }
}
