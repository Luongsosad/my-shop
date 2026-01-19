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

        private DashboardStats? _stats;
        public DashboardStats Stats
        {
            get => _stats ?? new DashboardStats
            {
                TotalProducts = 0,
                TodayOrders = 0,
                TodayRevenue = 0,
                LowStockCount = 0
            };
            set => SetProperty(ref _stats, value);
        }

        [ObservableProperty]
        private ObservableCollection<RecentOrderDto> recentOrders = new();

        [ObservableProperty]
        private ObservableCollection<LowStockProductDto> lowStockProducts = new();

        [ObservableProperty]
        private ObservableCollection<BestSellingProductDto> bestSellingProducts = new();

        [ObservableProperty]
        private ObservableCollection<DailyRevenueDto> monthlyRevenueData = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private DateTimeOffset _selectedDate = DateTimeOffset.Now;

        [ObservableProperty]
        private int _selectedMonth = DateTime.Now.Month;

        [ObservableProperty]
        private int _selectedYear = DateTime.Now.Year;

        public DashboardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            // Force initialize Stats
            _stats = new DashboardStats
            {
                TotalProducts = 0,
                TodayOrders = 0,
                TodayRevenue = 0,
                LowStockCount = 0
            };
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var statsTask = _dataService.GetDashboardStatsAsync();
                var ordersTask = _dataService.GetRecentOrdersAsync(3);
                var lowStockTask = _dataService.GetLowStockProductsAsync(5, 5);
                var bestSellingTask = _dataService.GetBestSellingProductsAsync(5);
                var revenueChartTask = _dataService.GetMonthlyRevenueChartAsync(SelectedYear, SelectedMonth);

                await Task.WhenAll(statsTask, ordersTask, lowStockTask, bestSellingTask, revenueChartTask);

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

                MonthlyRevenueData.Clear();
                var revenueDataList = await revenueChartTask;
                
                if (revenueDataList != null)
                {
                    foreach (var data in revenueDataList)
                    {
                        MonthlyRevenueData.Add(data);
                    }
                }
                
                OnPropertyChanged(nameof(MonthlyRevenueData));
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tải dữ liệu: {ex.Message}";
                
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

        [RelayCommand]
        public async Task LoadMonthlyRevenueAsync()
        {
            try
            {
                var revenueData = await _dataService.GetMonthlyRevenueChartAsync(SelectedYear, SelectedMonth);
                
                MonthlyRevenueData.Clear();
                if (revenueData != null)
                {
                    foreach (var data in revenueData)
                    {
                        MonthlyRevenueData.Add(data);
                    }
                }
                
                OnPropertyChanged(nameof(MonthlyRevenueData));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Không thể tải dữ liệu biểu đồ: {ex.Message}";
            }
        }

        public void UpdateSelectedDate(int year, int month)
        {
            SelectedYear = year;
            SelectedMonth = month;
            SelectedDate = new DateTimeOffset(new DateTime(year, month, 1));
        }
    }
}
