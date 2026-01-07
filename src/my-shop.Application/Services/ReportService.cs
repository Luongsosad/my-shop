using Microsoft.EntityFrameworkCore;
using my_shop.Application.DTOs;
using my_shop.Application.Interfaces;

namespace my_shop.Application.Services;

public class ReportService : IReportService
{
    private readonly IApplicationDbContext _context;

    public ReportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductSalesReportDto>> GetProductSalesReportAsync(ReportFilterDto filter, int topProducts = 10)
    {
        // Get order items within date range
        var orderItems = await _context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Product)
            .Where(oi => oi.Order.CreatedTime >= filter.StartDate && 
                        oi.Order.CreatedTime <= filter.EndDate)
            .ToListAsync();

        if (!orderItems.Any())
            return new List<ProductSalesReportDto>();

        // Group by product and get top sellers
        var productGroups = orderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Product = g.First().Product,
                TotalQuantity = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => (long)oi.Price * oi.Quantity),
                Items = g.ToList()
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(topProducts)
            .ToList();

        var result = new List<ProductSalesReportDto>();

        foreach (var group in productGroups)
        {
            var dataPoints = new List<ProductSalesDataPoint>();

            // Group by period based on filter type
            switch (filter.PeriodType)
            {
                case ReportPeriodType.Daily:
                    dataPoints = group.Items
                        .GroupBy(oi => oi.Order.CreatedTime.Date)
                        .Select(g => new ProductSalesDataPoint
                        {
                            Date = g.Key,
                            Period = g.Key.ToString("yyyy-MM-dd"),
                            QuantitySold = g.Sum(oi => oi.Quantity)
                        })
                        .OrderBy(x => x.Date)
                        .ToList();
                    break;

                case ReportPeriodType.Weekly:
                    dataPoints = group.Items
                        .GroupBy(oi => GetWeekOfYear(oi.Order.CreatedTime))
                        .Select(g => new ProductSalesDataPoint
                        {
                            Date = g.First().Order.CreatedTime.Date,
                            Period = $"Week {g.Key.Week}, {g.Key.Year}",
                            QuantitySold = g.Sum(oi => oi.Quantity)
                        })
                        .OrderBy(x => x.Date)
                        .ToList();
                    break;

                case ReportPeriodType.Monthly:
                    dataPoints = group.Items
                        .GroupBy(oi => new { oi.Order.CreatedTime.Year, oi.Order.CreatedTime.Month })
                        .Select(g => new ProductSalesDataPoint
                        {
                            Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                            Period = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                            QuantitySold = g.Sum(oi => oi.Quantity)
                        })
                        .OrderBy(x => x.Date)
                        .ToList();
                    break;

                case ReportPeriodType.Yearly:
                    dataPoints = group.Items
                        .GroupBy(oi => oi.Order.CreatedTime.Year)
                        .Select(g => new ProductSalesDataPoint
                        {
                            Date = new DateTime(g.Key, 1, 1),
                            Period = g.Key.ToString(),
                            QuantitySold = g.Sum(oi => oi.Quantity)
                        })
                        .OrderBy(x => x.Date)
                        .ToList();
                    break;
            }

            result.Add(new ProductSalesReportDto
            {
                ProductId = group.ProductId,
                ProductName = group.Product.Name,
                Sku = group.Product.Sku,
                TotalQuantitySold = group.TotalQuantity,
                TotalRevenue = group.TotalRevenue,
                DataPoints = dataPoints
            });
        }

        return result;
    }

    public async Task<RevenueProfitReportDto> GetRevenueProfitReportAsync(ReportFilterDto filter)
    {
        // Get orders within date range
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.CreatedTime >= filter.StartDate && 
                       o.CreatedTime <= filter.EndDate)
            .ToListAsync();

        if (!orders.Any())
        {
            return new RevenueProfitReportDto
            {
                TotalRevenue = 0,
                TotalProfit = 0,
                TotalCost = 0,
                TotalOrders = 0,
                AverageOrderValue = 0,
                ProfitMargin = 0,
                DataPoints = new List<RevenueProfitDataPoint>()
            };
        }

        // Calculate totals
        var totalRevenue = orders.Sum(o => (long)o.FinalPrice);
        var totalCost = orders.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity));
        var totalProfit = totalRevenue - totalCost;
        var totalOrders = orders.Count;
        var averageOrderValue = totalOrders > 0 ? (double)totalRevenue / totalOrders : 0;
        var profitMargin = totalRevenue > 0 ? (double)totalProfit / totalRevenue * 100 : 0;

        var dataPoints = new List<RevenueProfitDataPoint>();

        // Group by period based on filter type
        switch (filter.PeriodType)
        {
            case ReportPeriodType.Daily:
                dataPoints = orders
                    .GroupBy(o => o.CreatedTime.Date)
                    .Select(g => new RevenueProfitDataPoint
                    {
                        Date = g.Key,
                        Period = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Sum(o => (long)o.FinalPrice),
                        Cost = g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        Profit = g.Sum(o => (long)o.FinalPrice) - g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                break;

            case ReportPeriodType.Weekly:
                dataPoints = orders
                    .GroupBy(o => GetWeekOfYear(o.CreatedTime))
                    .Select(g => new RevenueProfitDataPoint
                    {
                        Date = g.First().CreatedTime.Date,
                        Period = $"Week {g.Key.Week}, {g.Key.Year}",
                        Revenue = g.Sum(o => (long)o.FinalPrice),
                        Cost = g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        Profit = g.Sum(o => (long)o.FinalPrice) - g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                break;

            case ReportPeriodType.Monthly:
                dataPoints = orders
                    .GroupBy(o => new { o.CreatedTime.Year, o.CreatedTime.Month })
                    .Select(g => new RevenueProfitDataPoint
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Period = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Revenue = g.Sum(o => (long)o.FinalPrice),
                        Cost = g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        Profit = g.Sum(o => (long)o.FinalPrice) - g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                break;

            case ReportPeriodType.Yearly:
                dataPoints = orders
                    .GroupBy(o => o.CreatedTime.Year)
                    .Select(g => new RevenueProfitDataPoint
                    {
                        Date = new DateTime(g.Key, 1, 1),
                        Period = g.Key.ToString(),
                        Revenue = g.Sum(o => (long)o.FinalPrice),
                        Cost = g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        Profit = g.Sum(o => (long)o.FinalPrice) - g.Sum(o => o.OrderItems.Sum(oi => (long)oi.Product.ImportPrice * oi.Quantity)),
                        OrderCount = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
                break;
        }

        return new RevenueProfitReportDto
        {
            TotalRevenue = totalRevenue,
            TotalProfit = totalProfit,
            TotalCost = totalCost,
            TotalOrders = totalOrders,
            AverageOrderValue = averageOrderValue,
            ProfitMargin = profitMargin,
            DataPoints = dataPoints
        };
    }

    private (int Year, int Week) GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var weekNum = culture.Calendar.GetWeekOfYear(date, 
            System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
            DayOfWeek.Monday);
        return (date.Year, weekNum);
    }
}
