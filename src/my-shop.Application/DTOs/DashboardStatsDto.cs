namespace my_shop.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TodayOrders { get; set; }
    public long TodayRevenue { get; set; }
    public int LowStockCount { get; set; }
}
