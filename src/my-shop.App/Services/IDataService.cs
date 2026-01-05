using my_shop.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace my_shop.App.Services
{
    public interface IDataService
    {
        // Dashboard operations
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 3);
        Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 5, int count = 5);
        Task<List<BestSellingProductDto>> GetBestSellingProductsAsync(int count = 5);
        
        // Product operations
        Task<List<ProductModel>> GetProductsAsync(string? searchTerm = null, int? categoryId = null);
        Task<ProductModel?> GetProductByIdAsync(int id);
        Task<ProductModel> CreateProductAsync(CreateProductModel model);
        Task<ProductModel> UpdateProductAsync(UpdateProductModel model);
        Task<bool> DeleteProductAsync(int id);
        
        // Category operations
        Task<List<CategoryModel>> GetCategoriesAsync();
        
    // Customer operations
    Task<List<CustomerModel>> GetCustomersAsync();
    Task<CustomerModel?> GetCustomerByIdAsync(int id);
    Task<CustomerModel> CreateCustomerAsync(string name, string phone);
    Task<CustomerModel> UpdateCustomerAsync(int id, string name, string phone);
    Task<bool> DeleteCustomerAsync(int id);
    
    // Order operations
    Task<OrderModel> CreateOrderAsync(CreateOrderModel model);
}
}
