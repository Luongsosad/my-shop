using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using my_shop.App.Models;
using my_shop.App.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class ReportScreenViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private DateTimeOffset startDate = DateTimeOffset.Now.AddDays(-30);

        [ObservableProperty]
        private DateTimeOffset endDate = DateTimeOffset.Now;

        [ObservableProperty]
        private ReportPeriodType selectedPeriodType;

        [ObservableProperty]
        private string totalRevenueText = "0 ₫";

        [ObservableProperty]
        private string totalProfitText = "0 ₫";

        [ObservableProperty]
        private string totalOrdersText = "0";

        [ObservableProperty]
        private string averageOrderValueText = "0 ₫";

        [ObservableProperty]
        private string profitMarginText = "0%";

        [ObservableProperty]
        private ObservableCollection<ISeries> productSalesSeriesCollection = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> revenueProfitSeriesCollection = new();

        [ObservableProperty]
        private IEnumerable<LiveChartsCore.Kernel.Sketches.ICartesianAxis>? productSalesXAxes;

        [ObservableProperty]
        private IEnumerable<LiveChartsCore.Kernel.Sketches.ICartesianAxis>? productSalesYAxes;

        [ObservableProperty]
        private IEnumerable<LiveChartsCore.Kernel.Sketches.ICartesianAxis>? revenueProfitXAxes;

        [ObservableProperty]
        private IEnumerable<LiveChartsCore.Kernel.Sketches.ICartesianAxis>? revenueProfitYAxes;

        public ObservableCollection<PeriodTypeItem> PeriodTypes { get; } = new()
        {
            new PeriodTypeItem { Name = "Daily", Type = ReportPeriodType.Daily },
            new PeriodTypeItem { Name = "Weekly", Type = ReportPeriodType.Weekly },
            new PeriodTypeItem { Name = "Monthly", Type = ReportPeriodType.Monthly },
            new PeriodTypeItem { Name = "Yearly", Type = ReportPeriodType.Yearly }
        };

        [ObservableProperty]
        private PeriodTypeItem? selectedPeriodItem;

        public ReportScreenViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            // Set default period to Daily
            SelectedPeriodItem = PeriodTypes.First(p => p.Type == ReportPeriodType.Daily);
            SelectedPeriodType = ReportPeriodType.Daily;
            
            System.Diagnostics.Debug.WriteLine("=== ReportScreenViewModel Constructor Called ===");
            System.Diagnostics.Debug.WriteLine($"DataService: {_dataService?.GetType().Name ?? "NULL"}");
            System.Diagnostics.Debug.WriteLine($"StartDate: {StartDate}");
            System.Diagnostics.Debug.WriteLine($"EndDate: {EndDate}");
            System.Diagnostics.Debug.WriteLine($"SelectedPeriodType: {SelectedPeriodType}");
            
            _ = LoadReportDataAsync();
        }

        partial void OnSelectedPeriodItemChanged(PeriodTypeItem? value)
        {
            if (value != null)
            {
                SelectedPeriodType = value.Type;
            }
        }

        [RelayCommand]
        private async Task LoadReportDataAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== LoadReportDataAsync Started ===");
            IsLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"Loading reports from {StartDate.Date:yyyy-MM-dd} to {EndDate.Date:yyyy-MM-dd}");
                
                // Load both reports in parallel
                var productSalesTask = _dataService.GetProductSalesReportAsync(StartDate.Date, EndDate.Date, SelectedPeriodType, 5);
                var revenueProfitTask = _dataService.GetRevenueProfitReportAsync(StartDate.Date, EndDate.Date, SelectedPeriodType);

                await Task.WhenAll(productSalesTask, revenueProfitTask);

                var productSalesData = await productSalesTask;
                var revenueProfitData = await revenueProfitTask;

                System.Diagnostics.Debug.WriteLine($"Product Sales Data Count: {productSalesData?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Revenue/Profit Data Points: {revenueProfitData?.DataPoints?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Total Revenue: {revenueProfitData?.TotalRevenue ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Total Profit: {revenueProfitData?.TotalProfit ?? 0}");

                UpdateSummaryCards(revenueProfitData);
                UpdateProductSalesChart(productSalesData);
                UpdateRevenueProfitChart(revenueProfitData);
                
                System.Diagnostics.Debug.WriteLine("=== LoadReportDataAsync Completed Successfully ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! ERROR loading report: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"!!! Stack Trace: {ex.StackTrace}");
                
                // Set default values on error
                TotalRevenueText = "Error";
                TotalProfitText = "Error";
                TotalOrdersText = "0";
            }
            finally
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine($"IsLoading set to false");
            }
        }

        [RelayCommand]
        private async Task ApplyFilterAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== ApplyFilter Clicked ===");
            await LoadReportDataAsync();
        }

        private void UpdateSummaryCards(RevenueProfitReport report)
        {
            if (report == null)
            {
                System.Diagnostics.Debug.WriteLine("!!! WARNING: RevenueProfitReport is null");
                return;
            }
            
            TotalRevenueText = FormatCurrency(report.TotalRevenue);
            TotalProfitText = FormatCurrency(report.TotalProfit);
            TotalOrdersText = report.TotalOrders.ToString("N0");
            AverageOrderValueText = FormatCurrency((long)report.AverageOrderValue);
            ProfitMarginText = $"{report.ProfitMargin:F2}%";
            
            System.Diagnostics.Debug.WriteLine($"Updated Summary Cards:");
            System.Diagnostics.Debug.WriteLine($"  Revenue: {TotalRevenueText}");
            System.Diagnostics.Debug.WriteLine($"  Profit: {TotalProfitText}");
            System.Diagnostics.Debug.WriteLine($"  Orders: {TotalOrdersText}");
        }

        private void UpdateProductSalesChart(List<ProductSalesReport> data)
        {
            System.Diagnostics.Debug.WriteLine($"=== UpdateProductSalesChart: {data?.Count ?? 0} products ===");
            
            var series = new List<ISeries>();

            if (data != null && data.Any())
            {
                // Define colors for each product series
                var colors = new[]
                {
                    SKColor.Parse("#FF6B6B"), // Red
                    SKColor.Parse("#4ECDC4"), // Teal
                    SKColor.Parse("#45B7D1"), // Blue
                    SKColor.Parse("#FFA07A"), // Light Salmon
                    SKColor.Parse("#98D8C8")  // Mint
                };

                // Get all unique periods from all products
                var allPeriods = data
                    .SelectMany(p => p.DataPoints.Select(dp => dp.Period))
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Unique periods count: {allPeriods.Count}");
                System.Diagnostics.Debug.WriteLine($"Periods: [{string.Join(", ", allPeriods)}]");

                if (allPeriods.Any())
                {
                    for (int i = 0; i < Math.Min(data.Count, 5); i++)
                    {
                        var product = data[i];
                        var color = colors[i % colors.Length];
                        
                        // Create a dictionary for quick lookup
                        var periodData = product.DataPoints.ToDictionary(dp => dp.Period, dp => dp.QuantitySold);
                        
                        // Fill values for all periods (0 if no data for that period)
                        var values = allPeriods.Select(period => 
                            periodData.ContainsKey(period) ? (double)periodData[period] : 0.0
                        ).ToArray();
                        
                        System.Diagnostics.Debug.WriteLine($"Product: {product.ProductName}, Values: [{string.Join(", ", values)}]");
                        
                        series.Add(new LineSeries<double>
                        {
                            Name = product.ProductName,
                            Values = values,
                            LineSmoothness = 0.5,
                            GeometrySize = 4, // Smaller points
                            GeometryFill = new SolidColorPaint(color), // Fill color for points
                            GeometryStroke = new SolidColorPaint(SKColors.White.WithAlpha(200)), // White border for points
                            Stroke = new SolidColorPaint(color.WithAlpha(180)), // Line color with some transparency
                            Fill = null
                        });
                    }

                    ProductSalesXAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = allPeriods.ToArray(),
                            LabelsRotation = -45,
                            TextSize = 12,
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100))
                        }
                    };

                    ProductSalesYAxes = new Axis[]
                    {
                        new Axis
                        {
                            Name = "Quantity Sold",
                            TextSize = 12,
                            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100))
                        }
                    };
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("!!! No product sales data to display");
            }

            ProductSalesSeriesCollection = new ObservableCollection<ISeries>(series);
            System.Diagnostics.Debug.WriteLine($"ProductSalesSeriesCollection updated with {series.Count} series");
        }

        private void UpdateRevenueProfitChart(RevenueProfitReport report)
        {
            System.Diagnostics.Debug.WriteLine($"=== UpdateRevenueProfitChart: {report?.DataPoints?.Count ?? 0} data points ===");
            
            var series = new List<ISeries>();

            if (report?.DataPoints != null && report.DataPoints.Any())
            {
                var labels = report.DataPoints.Select(dp => dp.Period).ToArray();
                var revenueValues = report.DataPoints.Select(dp => (double)dp.Revenue).ToArray();
                var profitValues = report.DataPoints.Select(dp => (double)dp.Profit).ToArray();

                System.Diagnostics.Debug.WriteLine($"Labels: [{string.Join(", ", labels)}]");
                System.Diagnostics.Debug.WriteLine($"Revenue: [{string.Join(", ", revenueValues)}]");
                System.Diagnostics.Debug.WriteLine($"Profit: [{string.Join(", ", profitValues)}]");

                series.Add(new ColumnSeries<double>
                {
                    Name = "Revenue",
                    Values = revenueValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#4CAF50")),
                    MaxBarWidth = 50
                });

                series.Add(new ColumnSeries<double>
                {
                    Name = "Profit",
                    Values = profitValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#2196F3")),
                    MaxBarWidth = 50
                });

                RevenueProfitXAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = labels,
                        LabelsRotation = -45,
                        TextSize = 12,
                        SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100))
                    }
                };

                RevenueProfitYAxes = new Axis[]
                {
                    new Axis
                    {
                        Name = "Amount (₫)",
                        TextSize = 12,
                        Labeler = value => FormatCurrency((long)value),
                        SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(100))
                    }
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("!!! No revenue/profit data to display");
            }

            RevenueProfitSeriesCollection = new ObservableCollection<ISeries>(series);
            System.Diagnostics.Debug.WriteLine($"RevenueProfitSeriesCollection updated with {series.Count} series");
        }

        private string FormatCurrency(long value)
        {
            return value.ToString("N0") + " ₫";
        }
    }

    public class PeriodTypeItem
    {
        public string Name { get; set; } = string.Empty;
        public ReportPeriodType Type { get; set; }
    }
}
