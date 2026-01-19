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
using Microsoft.UI.Xaml;

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
        private ObservableCollection<ISeries> revenueSparkline = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> profitSparkline = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> ordersSparkline = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> productSalesSeriesCollection = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> revenueProfitSeriesCollection = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> productRevenueDistributionSeries = new();

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
            IsLoading = true;

            try
            {
                var productSalesTask = _dataService.GetProductSalesReportAsync(StartDate.Date, EndDate.Date, SelectedPeriodType, 5);
                var revenueProfitTask = _dataService.GetRevenueProfitReportAsync(StartDate.Date, EndDate.Date, SelectedPeriodType);

                await Task.WhenAll(productSalesTask, revenueProfitTask);

                var productSalesData = await productSalesTask;
                var revenueProfitData = await revenueProfitTask;

                UpdateSummaryCards(revenueProfitData);
                UpdateSparklines(revenueProfitData);
                UpdateProductSalesChart(productSalesData);
                UpdateRevenueProfitChart(revenueProfitData);
                UpdateProductRevenueDistribution(productSalesData);
            }
            catch (Exception)
            {
                TotalRevenueText = "Error";
                TotalProfitText = "Error";
                TotalOrdersText = "0";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ApplyFilterAsync()
        {
            await LoadReportDataAsync();
        }

        [RelayCommand]
        private async Task SetDateRangeAsync(string range)
        {
            var now = DateTimeOffset.Now;
            
            switch (range.ToLower())
            {
                case "today":
                    StartDate = now.Date;
                    EndDate = now;
                    break;
                    
                case "yesterday":
                    StartDate = now.AddDays(-1).Date;
                    EndDate = now.AddDays(-1).Date.AddDays(1).AddTicks(-1);
                    break;
                    
                case "last7days":
                    StartDate = now.AddDays(-7).Date;
                    EndDate = now;
                    break;
                    
                case "last30days":
                    StartDate = now.AddDays(-30).Date;
                    EndDate = now;
                    break;
                    
                case "thismonth":
                    StartDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                    EndDate = now;
                    break;
                    
                case "lastmonth":
                    var lastMonth = now.AddMonths(-1);
                    StartDate = new DateTimeOffset(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0, now.Offset);
                    EndDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset).AddTicks(-1);
                    break;
                    
                case "thisyear":
                    StartDate = new DateTimeOffset(now.Year, 1, 1, 0, 0, 0, now.Offset);
                    EndDate = now;
                    break;
            }
            
            await LoadReportDataAsync();
        }

        private void UpdateSummaryCards(RevenueProfitReport report)
        {
            if (report == null)
            {
                return;
            }
            
            TotalRevenueText = FormatCurrency(report.TotalRevenue);
            TotalProfitText = FormatCurrency(report.TotalProfit);
            TotalOrdersText = report.TotalOrders.ToString("N0");
            AverageOrderValueText = FormatCurrency((long)report.AverageOrderValue);
            ProfitMarginText = $"{report.ProfitMargin:F2}%";
        }

        private void UpdateSparklines(RevenueProfitReport report)
        {
            if (report?.DataPoints == null || !report.DataPoints.Any())
            {
                RevenueSparkline = new ObservableCollection<ISeries>();
                ProfitSparkline = new ObservableCollection<ISeries>();
                OrdersSparkline = new ObservableCollection<ISeries>();
                return;
            }

            var revenueValues = report.DataPoints.Select(dp => (double)dp.Revenue).ToArray();
            var profitValues = report.DataPoints.Select(dp => (double)dp.Profit).ToArray();
            var orderValues = report.DataPoints.Select(dp => (double)dp.OrderCount).ToArray();

            RevenueSparkline = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = revenueValues,
                    Stroke = new SolidColorPaint(SKColor.Parse("#4CAF50")) { StrokeThickness = 2 },
                    Fill = new LinearGradientPaint(
                        new SKColor[] { SKColor.Parse("#4CAF50").WithAlpha(100), SKColors.Transparent },
                        new SKPoint(0, 0),
                        new SKPoint(0, 1)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                }
            };

            ProfitSparkline = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = profitValues,
                    Stroke = new SolidColorPaint(SKColor.Parse("#2196F3")) { StrokeThickness = 2 },
                    Fill = new LinearGradientPaint(
                        new SKColor[] { SKColor.Parse("#2196F3").WithAlpha(100), SKColors.Transparent },
                        new SKPoint(0, 0),
                        new SKPoint(0, 1)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                }
            };

            OrdersSparkline = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = orderValues,
                    Stroke = new SolidColorPaint(SKColor.Parse("#FF9800")) { StrokeThickness = 2 },
                    Fill = new LinearGradientPaint(
                        new SKColor[] { SKColor.Parse("#FF9800").WithAlpha(100), SKColors.Transparent },
                        new SKPoint(0, 0),
                        new SKPoint(0, 1)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                }
            };
        }

        private void UpdateProductSalesChart(List<ProductSalesReport> data)
        {
            var series = new List<ISeries>();

            if (data != null && data.Any())
            {
                var colors = new[]
                {
                    SKColor.Parse("#FF6B6B"),
                    SKColor.Parse("#4ECDC4"),
                    SKColor.Parse("#45B7D1"),
                    SKColor.Parse("#FFA07A"),
                    SKColor.Parse("#98D8C8")
                };

                var allPeriods = data
                    .SelectMany(p => p.DataPoints.Select(dp => dp.Period))
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                if (allPeriods.Any())
                {
                    for (int i = 0; i < Math.Min(data.Count, 5); i++)
                    {
                        var product = data[i];
                        var color = colors[i % colors.Length];
                        var periodData = product.DataPoints.ToDictionary(dp => dp.Period, dp => dp.QuantitySold);
                        var values = allPeriods.Select(period => 
                            periodData.ContainsKey(period) ? (double)periodData[period] : 0.0
                        ).ToArray();
                        
                        series.Add(new LineSeries<double>
                        {
                            Name = product.ProductName,
                            Values = values,
                            LineSmoothness = 0.5,
                            GeometrySize = 4,
                            GeometryFill = new SolidColorPaint(color),
                            GeometryStroke = new SolidColorPaint(SKColors.White.WithAlpha(200)),
                            Stroke = new SolidColorPaint(color.WithAlpha(180)),
                            Fill = null
                        });
                    }

                    var textColor = GetChartTextColor();
                    var separatorColor = GetChartSeparatorColor();

                    ProductSalesXAxes = new Axis[]
                    {
                        new Axis
                        {
                            Labels = allPeriods.ToArray(),
                            LabelsRotation = -45,
                            TextSize = 12,
                            LabelsPaint = new SolidColorPaint(textColor),
                            SeparatorsPaint = new SolidColorPaint(separatorColor)
                        }
                    };

                    ProductSalesYAxes = new Axis[]
                    {
                        new Axis
                        {
                            Name = "Quantity Sold",
                            TextSize = 12,
                            LabelsPaint = new SolidColorPaint(textColor),
                            NamePaint = new SolidColorPaint(textColor),
                            SeparatorsPaint = new SolidColorPaint(separatorColor)
                        }
                    };
                }
            }

            ProductSalesSeriesCollection = new ObservableCollection<ISeries>(series);
        }

        private void UpdateRevenueProfitChart(RevenueProfitReport report)
        {
            var series = new List<ISeries>();

            if (report?.DataPoints != null && report.DataPoints.Any())
            {
                var labels = report.DataPoints.Select(dp => dp.Period).ToArray();
                var revenueValues = report.DataPoints.Select(dp => (double)dp.Revenue).ToArray();
                var profitValues = report.DataPoints.Select(dp => (double)dp.Profit).ToArray();

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

                var textColor = GetChartTextColor();
                var separatorColor = GetChartSeparatorColor();

                RevenueProfitXAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = labels,
                        LabelsRotation = -45,
                        TextSize = 12,
                        LabelsPaint = new SolidColorPaint(textColor),
                        SeparatorsPaint = new SolidColorPaint(separatorColor)
                    }
                };

                RevenueProfitYAxes = new Axis[]
                {
                    new Axis
                    {
                        Name = "Amount (₫)",
                        TextSize = 12,
                        Labeler = value => FormatCurrency((long)value),
                        LabelsPaint = new SolidColorPaint(textColor),
                        NamePaint = new SolidColorPaint(textColor),
                        SeparatorsPaint = new SolidColorPaint(separatorColor)
                    }
                };
            }

            RevenueProfitSeriesCollection = new ObservableCollection<ISeries>(series);
        }

        private void UpdateProductRevenueDistribution(List<ProductSalesReport> data)
        {
            var series = new List<ISeries>();

            if (data != null && data.Any())
            {
                // Calculate total revenue for each product
                var productRevenues = data.Select(p => new
                {
                    ProductName = p.ProductName,
                    TotalRevenue = p.TotalRevenue
                }).OrderByDescending(p => p.TotalRevenue).ToList();

                var totalRevenue = productRevenues.Sum(p => p.TotalRevenue);
                
                // Take top 5 products
                var topProducts = productRevenues.Take(5).ToList();
                var othersRevenue = productRevenues.Skip(5).Sum(p => p.TotalRevenue);

                var colors = new[]
                {
                    SKColor.Parse("#FF6B6B"), // Red
                    SKColor.Parse("#4ECDC4"), // Teal
                    SKColor.Parse("#45B7D1"), // Blue
                    SKColor.Parse("#FFA07A"), // Light Salmon
                    SKColor.Parse("#98D8C8"), // Mint
                    SKColor.Parse("#95A5A6")  // Gray for Others
                };

                int colorIndex = 0;
                foreach (var product in topProducts)
                {
                    var percentage = totalRevenue > 0 ? (double)product.TotalRevenue / totalRevenue * 100 : 0;
                    
                    series.Add(new PieSeries<double>
                    {
                        Name = $"{product.ProductName} ({percentage:F1}%)",
                        Values = new[] { (double)product.TotalRevenue },
                        Fill = new SolidColorPaint(colors[colorIndex % colors.Length]),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsSize = 12,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{percentage:F1}%"
                    });
                    
                    colorIndex++;
                }

                // Add "Others" if there are more than 5 products
                if (othersRevenue > 0)
                {
                    var othersPercentage = totalRevenue > 0 ? (double)othersRevenue / totalRevenue * 100 : 0;
                    
                    series.Add(new PieSeries<double>
                    {
                        Name = $"Others ({othersPercentage:F1}%)",
                        Values = new[] { (double)othersRevenue },
                        Fill = new SolidColorPaint(colors[colorIndex % colors.Length]),
                        DataLabelsPaint = new SolidColorPaint(SKColors.White),
                        DataLabelsSize = 12,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{othersPercentage:F1}%"
                    });
                }
            }

            ProductRevenueDistributionSeries = new ObservableCollection<ISeries>(series);
        }

        private string FormatCurrency(long value)
        {
            return value.ToString("N0") + " ₫";
        }

        private bool IsDarkTheme()
        {
            try
            {
                if (MainWindow.Instance?.Content is FrameworkElement rootElement)
                {
                    return rootElement.ActualTheme == ElementTheme.Dark;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        private SKColor GetChartTextColor()
        {
            return IsDarkTheme() 
                ? SKColor.Parse("#E0E0E0")
                : SKColor.Parse("#424242");
        }

        private SKColor GetChartSeparatorColor()
        {
            return IsDarkTheme()
                ? SKColors.LightGray.WithAlpha(80)
                : SKColors.LightGray.WithAlpha(100);
        }
    }

    public class PeriodTypeItem
    {
        public string Name { get; set; } = string.Empty;
        public ReportPeriodType Type { get; set; }
    }
}
