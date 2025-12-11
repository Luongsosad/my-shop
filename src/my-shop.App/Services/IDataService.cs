using my_shop.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public interface IDataService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 3);
        Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 5, int count = 5);
        Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 5);
    }
}
