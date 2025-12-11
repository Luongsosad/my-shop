using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using my_shop.App.Models;
using my_shop.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private DashboardStats? stats;

        [ObservableProperty]
        private ObservableCollection<RecentOrderDto> recentOrders = new();

        [ObservableProperty]
        private ObservableCollection<LowStockProductDto> lowStockProducts = new();

        [ObservableProperty]
        private ObservableCollection<BestSellingProductDto> bestSellingProducts = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasError;

        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                // Load all dashboard data
                var statsTask = _dataService.GetDashboardStatsAsync();
                var ordersTask = _dataService.GetRecentOrdersAsync(3);
                var lowStockTask = _dataService.GetLowStockProductsAsync(5, 5);
                var bestSellingTask = _dataService.GetBestSellingProductsAsync(5);

                await Task.WhenAll(statsTask, ordersTask, lowStockTask, bestSellingTask);

                Stats = await statsTask;
                
                RecentOrders.Clear();
                foreach (var order in await ordersTask)
                {
                    RecentOrders.Add(order);
                }

                LowStockProducts.Clear();
                foreach (var product in await lowStockTask)
                {
                    LowStockProducts.Add(product);
                }

                BestSellingProducts.Clear();
                foreach (var product in await bestSellingTask)
                {
                    BestSellingProducts.Add(product);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tải dữ liệu: {ex.Message}";
                
                // Set default values on error
                Stats = new DashboardStats
                {
                    TotalProducts = 0,
                    TodayOrders = 0,
                    TodayRevenue = 0,
                    LowStockCount = 0
                };
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
