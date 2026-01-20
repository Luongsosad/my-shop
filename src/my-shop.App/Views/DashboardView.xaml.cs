using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using my_shop.App.ViewModels;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

namespace my_shop.App.Views
{
    public sealed partial class DashboardView : Page
    {
        public DashboardViewModel ViewModel { get; }
        private List<DailyRevenueDto> _chartData = new();
        private double _chartWidth;
        private double _chartHeight;
        private int _currentRank = 0;

        public DashboardView()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: Constructor START");
                
                this.InitializeComponent();
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: InitializeComponent DONE");
                
                // CRITICAL: Get ViewModel AFTER InitializeComponent
                ViewModel = App.GetService<DashboardViewModel>();
                System.Diagnostics.Debug.WriteLine($"🔵 DashboardView: ViewModel created. Stats null? {ViewModel.Stats == null}");
                
                // Set DataContext explicitly
                this.DataContext = ViewModel;
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: DataContext set");
                
                this.Loaded += DashboardView_Loaded;
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                
                InitializeYearComboBox();
                InitializeMonthComboBox();
                
                // Add SizeChanged event to redraw chart when canvas size changes
                this.SizeChanged += DashboardView_SizeChanged;
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: Constructor END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DashboardView Constructor Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private void InitializeYearComboBox()
        {
            System.Diagnostics.Debug.WriteLine("[Dashboard] InitializeYearComboBox START");
            // Populate years from 2020 to current year + 1
            var currentYear = DateTime.Now.Year;
            for (int year = 2020; year <= currentYear + 1; year++)
            {
                YearComboBox.Items.Add(year);
            }
            
            // Set current year as selected
            YearComboBox.SelectedItem = currentYear;
            System.Diagnostics.Debug.WriteLine($"[Dashboard] YearComboBox initialized with {YearComboBox.Items.Count} years, selected: {currentYear}");
        }

        private void InitializeMonthComboBox()
        {
            System.Diagnostics.Debug.WriteLine("[Dashboard] InitializeMonthComboBox START");
            // Set current month as selected (index = month - 1)
            MonthComboBox.SelectedIndex = DateTime.Now.Month - 1;
            System.Diagnostics.Debug.WriteLine($"[Dashboard] MonthComboBox selected index: {MonthComboBox.SelectedIndex} (Month {DateTime.Now.Month})");
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[Dashboard] DashboardView_Loaded START");
            await ViewModel.LoadDataAsync();
            System.Diagnostics.Debug.WriteLine("[Dashboard] DashboardView_Loaded END");
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[Dashboard] PropertyChanged: {e.PropertyName}");
            if (e.PropertyName == nameof(ViewModel.MonthlyRevenueData))
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] MonthlyRevenueData changed, count: {ViewModel.MonthlyRevenueData?.Count ?? 0}");
                DrawRevenueChart();
            }
        }

        private async void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthComboBox.SelectedItem is ComboBoxItem selectedItem && 
                selectedItem.Tag is string monthTag)
            {
                var month = int.Parse(monthTag);
                var year = YearComboBox.SelectedItem as int? ?? DateTime.Now.Year;
                
                ViewModel.UpdateSelectedDate(year, month);
                await ViewModel.LoadMonthlyRevenueAsync();
            }
        }

        private async void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearComboBox.SelectedItem is int year)
            {
                var monthItem = MonthComboBox.SelectedItem as ComboBoxItem;
                var month = monthItem != null && monthItem.Tag is string tag 
                    ? int.Parse(tag) 
                    : DateTime.Now.Month;
                
                ViewModel.UpdateSelectedDate(year, month);
                await ViewModel.LoadMonthlyRevenueAsync();
            }
        }

        private void RevenueChartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_chartData == null || !_chartData.Any()) return;

            var canvas = sender as Canvas;
            if (canvas == null) return;

            var point = e.GetCurrentPoint(canvas).Position;
            
            // Find nearest data point
            var dataPoint = FindNearestDataPoint(point);
            if (dataPoint != null)
            {
                ShowTooltip(dataPoint, point);
            }
        }

        private void RevenueChartCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ChartTooltipPopup.IsOpen = false;
        }

        private DailyRevenueDto FindNearestDataPoint(Point mousePoint)
        {
            if (_chartData == null || !_chartData.Any()) return null;

            var padding = 40.0;
            var availableWidth = _chartWidth - padding * 2;
            var columnWidth = availableWidth / _chartData.Count;

            // Calculate which column the mouse is over
            var mouseX = mousePoint.X - padding;
            if (mouseX < 0 || mouseX > availableWidth) return null;

            var index = (int)(mouseX / columnWidth);
            if (index < 0 || index >= _chartData.Count) return null;

            return _chartData[index];
        }

        private void ShowTooltip(DailyRevenueDto dataPoint, Point position)
        {
            // Update tooltip content
            TooltipDate.Text = $"Day {dataPoint.Day}";
            TooltipRevenue.Text = $"{dataPoint.Revenue:N0}₫";
            TooltipOrders.Text = $"{dataPoint.OrderCount} orders";

            // Position tooltip
            ChartTooltipPopup.HorizontalOffset = position.X + 10;
            ChartTooltipPopup.VerticalOffset = position.Y - 60;
            
            // Show tooltip
            ChartTooltipPopup.IsOpen = true;
        }

        private void DrawRevenueChart()
        {
            System.Diagnostics.Debug.WriteLine($"[Dashboard] DrawRevenueChart called");
            
            if (RevenueChartCanvas == null)
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] ❌ RevenueChartCanvas is null");
                return;
            }
                
            if (RevenueChartCanvas.ActualWidth == 0 || RevenueChartCanvas.ActualHeight == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] ⚠️ Canvas size: {RevenueChartCanvas.ActualWidth}x{RevenueChartCanvas.ActualHeight}");
                return;
            }

            var data = ViewModel.MonthlyRevenueData;
            if (data == null || !data.Any())
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] ⚠️ No data to display. Data null: {data == null}, Count: {data?.Count ?? 0}");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[Dashboard] ✅ Drawing chart with {data.Count} data points");

            // Store for tooltip calculations
            _chartData = data.ToList();
            _chartWidth = RevenueChartCanvas.ActualWidth;
            _chartHeight = RevenueChartCanvas.ActualHeight;

            RevenueChartCanvas.Children.Clear();

            var width = RevenueChartCanvas.ActualWidth;
            var height = RevenueChartCanvas.ActualHeight;
            var padding = 40.0;
            var chartWidth = width - padding * 2;
            var chartHeight = height - padding * 2;

            if (chartWidth <= 0 || chartHeight <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"[Dashboard] ❌ Invalid chart dimensions: {chartWidth}x{chartHeight}");
                return;
            }

            var maxRevenue = data.Max(d => d.Revenue);
            if (maxRevenue == 0) maxRevenue = 1;

            var maxOrders = data.Max(d => d.OrderCount);
            if (maxOrders == 0) maxOrders = 1;

            System.Diagnostics.Debug.WriteLine($"[Dashboard] Max Revenue: {maxRevenue:N0}, Max Orders: {maxOrders}");

            var barWidth = Math.Max(8, chartWidth / data.Count / 2 - 4);
            var groupSpacing = chartWidth / data.Count;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var x = padding + i * groupSpacing;

                var revenueHeight = (item.Revenue / (double)maxRevenue) * chartHeight;
                var orderHeight = (item.OrderCount / (double)maxOrders) * chartHeight;

                var revenueBar = new Rectangle
                {
                    Width = barWidth,
                    Height = Math.Max(2, revenueHeight),
                    Fill = new SolidColorBrush(Microsoft.UI.Colors.DodgerBlue),
                    RadiusX = 4,
                    RadiusY = 4
                };
                Canvas.SetLeft(revenueBar, x);
                Canvas.SetTop(revenueBar, padding + chartHeight - Math.Max(2, revenueHeight));
                RevenueChartCanvas.Children.Add(revenueBar);

                var orderBar = new Rectangle
                {
                    Width = barWidth,
                    Height = Math.Max(2, orderHeight),
                    Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 16, 185, 129)),
                    RadiusX = 4,
                    RadiusY = 4
                };
                Canvas.SetLeft(orderBar, x + barWidth + 4);
                Canvas.SetTop(orderBar, padding + chartHeight - Math.Max(2, orderHeight));
                RevenueChartCanvas.Children.Add(orderBar);

                if (i % 5 == 0 || i == data.Count - 1)
                {
                    var dayLabel = new TextBlock
                    {
                        Text = item.Day.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Microsoft.UI.Colors.Gray)
                    };
                    Canvas.SetLeft(dayLabel, x + barWidth / 2 - 6);
                    Canvas.SetTop(dayLabel, padding + chartHeight + 8);
                    RevenueChartCanvas.Children.Add(dayLabel);
                }
            }

            var yAxisLine = new Line
            {
                X1 = padding,
                Y1 = padding,
                X2 = padding,
                Y2 = padding + chartHeight,
                Stroke = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                StrokeThickness = 1
            };
            RevenueChartCanvas.Children.Add(yAxisLine);

            var xAxisLine = new Line
            {
                X1 = padding,
                Y1 = padding + chartHeight,
                X2 = padding + chartWidth,
                Y2 = padding + chartHeight,
                Stroke = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
                StrokeThickness = 1
            };
            RevenueChartCanvas.Children.Add(xAxisLine);
            
            System.Diagnostics.Debug.WriteLine($"[Dashboard] ✅ Chart drawn successfully with {RevenueChartCanvas.Children.Count} elements");
        }

        private void DashboardView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Redraw chart when page size changes
            if (ViewModel.MonthlyRevenueData != null && ViewModel.MonthlyRevenueData.Any())
            {
                DrawRevenueChart();
            }
        }

        private void ProductCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border)
            {
                _currentRank++;

                // Find the RankText TextBlock
                var rankText = FindChildByName(border, "RankText") as TextBlock;
                if (rankText != null)
                {
                    rankText.Text = _currentRank.ToString();
                }

                // Find the RankBadge Border and update gradient based on rank
                var rankBadge = FindChildByName(border, "RankBadge") as Border;
                if (rankBadge != null)
                {
                    rankBadge.Background = GetRankGradient(_currentRank);
                }
            }
        }

        private Brush GetRankGradient(int rank)
        {
            return rank switch
            {
                1 => new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = Color.FromArgb(255, 251, 191, 36), Offset = 0 },  // Gold
                new GradientStop { Color = Color.FromArgb(255, 217, 119, 6), Offset = 1 }
            }
                },
                2 => new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = Color.FromArgb(255, 203, 213, 225), Offset = 0 },  // Silver
                new GradientStop { Color = Color.FromArgb(255, 148, 163, 184), Offset = 1 }
            }
                },
                3 => new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = Color.FromArgb(255, 217, 119, 6), Offset = 0 },   // Bronze
                new GradientStop { Color = Color.FromArgb(255, 146, 64, 14), Offset = 1 }
            }
                },
                _ => new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = Color.FromArgb(255, 99, 102, 241), Offset = 0 },   // Purple
                new GradientStop { Color = Color.FromArgb(255, 79, 70, 229), Offset = 1 }
            }
                }
            };
        }

        private FrameworkElement? FindChildByName(DependencyObject parent, string name)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement element && element.Name == name)
                {
                    return element;
                }

                var result = FindChildByName(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        #region Stat Card Hover Animations

        private void StatCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                var scaleAnimation = new DoubleAnimation
                {
                    To = 1.05,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                var storyboard = new Storyboard();
                
                Storyboard.SetTarget(scaleAnimation, border);
                Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                storyboard.Children.Add(scaleAnimation);

                var scaleYAnimation = new DoubleAnimation
                {
                    To = 1.05,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                
                Storyboard.SetTarget(scaleYAnimation, border);
                Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                storyboard.Children.Add(scaleYAnimation);

                storyboard.Begin();

                border.Translation = new System.Numerics.Vector3(0, -4, 32);
            }
        }

        private void StatCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border)
            {
                var scaleAnimation = new DoubleAnimation
                {
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                var storyboard = new Storyboard();
                
                Storyboard.SetTarget(scaleAnimation, border);
                Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                storyboard.Children.Add(scaleAnimation);

                var scaleYAnimation = new DoubleAnimation
                {
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                
                Storyboard.SetTarget(scaleYAnimation, border);
                Storyboard.SetTargetProperty(scaleYAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                storyboard.Children.Add(scaleYAnimation);

                storyboard.Begin();

                border.Translation = new System.Numerics.Vector3(0, 0, 0);
            }
        }

        #endregion

        #region Stat Card Click Actions

        private async void ProductCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Products Overview",
                Content = $"Total Products: {ViewModel.Stats.TotalProducts}\n\nClick OK to navigate to Products page.",
                PrimaryButtonText = "Go to Products",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Navigate to Products page");
            }
        }

        private async void OrderCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Today's Orders",
                Content = $"Orders Today: {ViewModel.Stats.TodayOrders}\n\nClick OK to view all orders.",
                PrimaryButtonText = "View Orders",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Navigate to Orders page");
            }
        }

        private async void RevenueCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Revenue Details",
                Content = $"Today's Revenue: {ViewModel.Stats.TodayRevenue:N0} đ\n\nView detailed revenue analytics.",
                PrimaryButtonText = "View Analytics",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Navigate to Revenue Analytics");
            }
        }

        private async void LowStockCard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Low Stock Alert",
                Content = $"Items Running Low: {ViewModel.Stats.LowStockCount}\n\nClick OK to view and manage low stock items.",
                PrimaryButtonText = "Manage Inventory",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Navigate to Low Stock Management");
            }
        }

        #endregion

        // ...existing chart methods...
    }
}
