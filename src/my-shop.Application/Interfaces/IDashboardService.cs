using my_shop.Application.DTOs;
using my_shop.Domain.Entities;

namespace my_shop.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 3);
    Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 5, int count = 5);
    Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 5);
    Task<List<DailyRevenueDto>> GetMonthlyRevenueChartAsync(int year, int month);
}
