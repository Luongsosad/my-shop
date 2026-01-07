using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using my_shop.App.Models;
using my_shop.App.Services.OrdersService;
using my_shop.App.Services.PdfService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;


namespace my_shop.App.ViewModels
{
    public partial class OrdersScreenViewModel : ObservableObject
    {
        public readonly OrderService _orderService;
        private readonly OrderPdfService _pdfService;

        [ObservableProperty]
        private ObservableCollection<Models.Order> orders = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 10;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private int totalOrders;

        [ObservableProperty]
        private DateTimeOffset? searchFromDate;

        [ObservableProperty]
        private DateTimeOffset? searchToDate;

        [ObservableProperty]
        private Customer? selectedCustomer;

        [ObservableProperty]
        private Models.Product? selectedProduct;

        [ObservableProperty]
        private double searchMinPrice = double.NaN;

        [ObservableProperty]
        private double searchMaxPrice = double.NaN;

        [ObservableProperty]
        private string? selectedStatus;

        [ObservableProperty]
        private bool isSearchExpanded = false;

        public XamlRoot? XamlRoot { get; set; }

        public OrdersScreenViewModel(OrderService orderService)
        {
            _orderService = orderService;
            _pdfService = new OrderPdfService();
        }

        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchExpanded = !IsSearchExpanded;
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true;

                var (ordersList, totalCount) = await _orderService.GetOrdersAsync(CurrentPage, PageSize);

                Orders.Clear();
                foreach (var order in ordersList)
                {
                    Orders.Add(order);
                }

                TotalOrders = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ToggleExpand(Models.Order order)
        {
            order.IsExpanded = !order.IsExpanded;
            OnPropertyChanged(nameof(Orders));
        }

        [RelayCommand]
        private async Task MarkAsPaidAsync(Models.Order order)
        {
            try
            {
                IsLoading = true;

                await _orderService.UpdateOrderStatusAsync(order.Id, "paid");
                order.Status = "paid";
                OnPropertyChanged(nameof(Orders));

                await ShowSuccessAsync("Order has just paid!");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelOrderAsync(Models.Order order)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirm cancel",
                Content = $"Are you sure to cancel Order ID #{order.Id}?",
                PrimaryButtonText = "Confirm",
                CloseButtonText = "No",
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            try
            {
                IsLoading = true;

                await _orderService.UpdateOrderStatusAsync(order.Id, "cancelled");
                order.Status = "cancelled";
                OnPropertyChanged(nameof(Orders));

                await ShowSuccessAsync("Order has just cancelled!");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ShowCustomerDetailAsync(Customer customer)
        {
            if (XamlRoot == null) return;

            var dialog = new ContentDialog
            {
                Title = "Customer Details",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot,
                RequestedTheme = ElementTheme.Light,
                DefaultButton = ContentDialogButton.Close
            };

            var content = CreateCustomerDetailContent(customer);
            dialog.Content = content;

            await dialog.ShowAsync();
        }

        [RelayCommand]
        private async Task ShowProductDetailAsync(Models.Product product)
        {
            if (XamlRoot == null) return;

            var dialog = new ContentDialog
            {
                Title = "Product Details",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot,
                RequestedTheme = ElementTheme.Light,
                DefaultButton = ContentDialogButton.Close
            };

            Debug.WriteLine("Debug 1");

            var scrollViewer = new ScrollViewer
            {
                MaxHeight = 500,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
            };

            Debug.WriteLine("Debug 2");

            var content = CreateProductDetailContent(product);
            scrollViewer.Content = content;
            dialog.Content = scrollViewer;

            Debug.WriteLine("Debug 3");

            await dialog.ShowAsync();
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadOrdersAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadOrdersAsync();
            }
        }

        private async Task ShowSuccessAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowErrorAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Something went wrong. Please try again!",
                Content = message,
                CloseButtonText = "OK",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            if (XamlRoot == null) return false;

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Xác nhận",
                CloseButtonText = "Hủy",
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        // Helper methods for creating dialog content
        private StackPanel CreateCustomerDetailContent(Customer customer)
        {
            var content = new StackPanel
            {
                Spacing = 16,
                MinWidth = 400,
                RequestedTheme = ElementTheme.Light,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.White
                ),   
            };

            // Header với icon
            var header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 12,
                Margin = new Thickness(0, 0, 0, 16)
            };
            header.Children.Add(new FontIcon
            {
                Glyph = "\uE77B",
                FontSize = 24,
            });
            header.Children.Add(new TextBlock
            {
                Text = customer.Name,
                FontSize = 24,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiLight,
                VerticalAlignment = VerticalAlignment.Center
            });
            content.Children.Add(header);

            // Divider
            //content.Children.Add(CreateDivider());

            // Customer ID
            AddInfoRow(content, "Customer ID:", $"#{customer.Id}");

            // Phone
            AddInfoRow(content, "Phone Number:", customer.Phone);

            // Created Date
            AddInfoRow(content, "Member Since:", customer.CreatedAt.ToString("dd MMM yyyy, HH:mm"));

            // Status
            var statusStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            statusStack.Children.Add(new TextBlock
            {
                Text = "Status:",
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Black
                ),
            });

            var statusBorder = new Border
            {
                Background = customer.IsDeleted
                    ? GetResourceBrush("SystemFillColorCriticalBrush")
                    : GetResourceBrush("SystemFillColorSuccessBrush"),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 4, 12, 4)
            };
            statusBorder.Child = new TextBlock
            {
                Text = customer.IsDeleted ? "Inactive" : "Active",
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
            };
            statusStack.Children.Add(statusBorder);
            content.Children.Add(statusStack);

            // Total Orders
            if (customer.Orders?.Any() == true)
            {
                content.Children.Add(CreateDivider(8));
                AddInfoRow(content, "Total Orders:", customer.Orders.Count.ToString());
            }

            return content;
        }

        private StackPanel CreateProductDetailContent(Models.Product product)
        {
            var content = new StackPanel
            {
                Spacing = 16,
                MinWidth = 450,
                Padding = new Microsoft.UI.Xaml.Thickness(10),
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
            };

            try
            {
                // Header
                var header = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 12,
                };

                header.Children.Add(new FontIcon
                {
                    Glyph = "\uE7B8",
                    FontSize = 32,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DodgerBlue)
                });

                header.Children.Add(new TextBlock
                {
                    Text = product.Name ?? "N/A",
                    FontSize = 24,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black)
                });

                content.Children.Add(header);

                if (product.Images != null && product.Images.Count > 0)
                {
                    try
                    {
                        var imagesGrid = new Grid
                        {
                            ColumnSpacing = 8,
                            RowSpacing = 8
                        };

                        var imagesToShow = product.Images.Take(4).ToList();
                        for (int i = 0; i < imagesToShow.Count; i++)
                        {
                            imagesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                            var imageBorder = new Border
                            {
                                CornerRadius = new CornerRadius(8),
                                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                                BorderThickness = new Thickness(1)
                            };

                            try
                            {
                                var imageUrl = imagesToShow[i];

                                // Validate URL
                                if (!string.IsNullOrWhiteSpace(imageUrl) && Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                                {
                                    var image = new Image
                                    {
                                        Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(uri),
                                        Stretch = Microsoft.UI.Xaml.Media.Stretch.UniformToFill
                                    };
                                    imageBorder.Child = image;
                                }
                                else
                                {
                                    throw new ArgumentException("Invalid image URL");
                                }
                            }
                            catch (Exception imgEx)
                            {
                                Debug.WriteLine($"Error loading image {i}: {imgEx.Message}");

                                // Placeholder icon
                                var icon = new FontIcon
                                {
                                    Glyph = "\uE91B",
                                    FontSize = 24,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray)
                                };
                                imageBorder.Child = icon;
                            }

                            Grid.SetColumn(imageBorder, i);
                            imagesGrid.Children.Add(imageBorder);
                        }

                        content.Children.Add(imagesGrid);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error creating images section: {ex.Message}");
                    }
                }

                // Divider
                content.Children.Add(CreateDivider());

                // Product info
                AddInfoRow(content, "Product ID:", $"#{product.Id}");
                AddInfoRow(content, "SKU Code:", product.Sku ?? "N/A");

                if (product.Category != null)
                {
                    AddInfoRow(content, "Category:", product.Category.Name ?? "N/A");
                }

                AddInfoRow(content, "Import Price:", $"{product.ImportPrice:N0} đ");

                // Stock status
                var stockStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
                stockStack.Children.Add(new TextBlock
                {
                    Text = "Stock:",
                    FontSize = 12,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black)
                });

                var stockBorder = new Border
                {
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        product.Count > 0 ? Microsoft.UI.Colors.ForestGreen : Microsoft.UI.Colors.Crimson
                    ),
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(12, 4, 12, 4)
                };

                stockBorder.Child = new TextBlock
                {
                    Text = product.Count > 0 ? $"{product.Count} units" : "Out of stock",
                    FontSize = 12,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                };

                stockStack.Children.Add(stockBorder);
                content.Children.Add(stockStack);

                // Description
                if (!string.IsNullOrEmpty(product.Description))
                {
                    content.Children.Add(new TextBlock
                    {
                        Text = "Description:",
                        FontSize = 12,
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black)
                    });

                    content.Children.Add(new TextBlock
                    {
                        Text = product.Description,
                        FontSize = 12,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DimGray)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateProductDetailContent: {ex.Message}");

                // Add error message to UI
                content.Children.Add(new TextBlock
                {
                    Text = $"Error loading product details: {ex.Message}",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                    TextWrapping = TextWrapping.Wrap
                });
            }

            return content;
        }

        private void AddInfoRow(StackPanel parent, string label, string value)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };

            stack.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Black
                ),
                MinWidth = 120
            });

            stack.Children.Add(new TextBlock
            {
                Text = value,
                FontSize = 12,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Black
                ),
                TextWrapping = TextWrapping.Wrap
            });

            parent.Children.Add(stack);
        }

        private Border CreateDivider(double marginVertical = 0)
        {
            return new Border
            {
                Height = 1,
                Background = GetResourceBrush("DividerStrokeColorDefaultBrush"),
                Margin = new Thickness(0, marginVertical, 0, marginVertical)
            };
        }

        private Microsoft.UI.Xaml.Media.Brush GetResourceBrush(string key)
        {
            return (Microsoft.UI.Xaml.Media.Brush)
                Microsoft.UI.Xaml.Application.Current.Resources[key];
        }


































        [RelayCommand]
        private async Task EditOrderAsync(Models.Order order)
        {
            if (XamlRoot == null) return;

            var editableItems = new ObservableCollection<EditableOrderItem>();

            // Load existing items
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    editableItems.Add(new EditableOrderItem(item, () => { }));
                }
            }

            var dialog = new ContentDialog
            {
                PrimaryButtonText = "Save Changes",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot,
                RequestedTheme = ElementTheme.Light,
                Width = 2000,
            };

            dialog.Resources["ContentDialogMaxWidth"] = 2000d;
            dialog.Resources["ContentDialogMinWidth"] = 1000d;

            var content = CreateEditOrderContent(order, editableItems, dialog);

            dialog.Content = content;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await SaveOrderChangesAsync(order, editableItems);
            }
        }

        private ScrollViewer CreateEditOrderContent(
            Models.Order order,
            ObservableCollection<EditableOrderItem> editableItems,
            ContentDialog parentDialog)
        {
            var baseBrush = GetResourceBrush("LayerFillColorDefaultBrush") as SolidColorBrush;
            var baseColor = baseBrush.Color;

            var scrollViewer = new ScrollViewer
            {
                MinWidth = 500,
                MaxHeight = 650,
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                RequestedTheme = ElementTheme.Light
            };

            var mainStack = new StackPanel
            {
                Spacing = 14,
                Padding = new Thickness(8)
            };

            // Header with Total
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var headerStack = new StackPanel { Spacing = 4 };
            headerStack.Children.Add(new TextBlock
            {
                Text = $"Order #{order.Id}",
                FontSize = 20,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            headerStack.Children.Add(new TextBlock
            {
                Text = order.FormattedDate,
                Opacity = 0.6,
                FontSize = 12,
            });
            Grid.SetColumn(headerStack, 0);
            headerGrid.Children.Add(headerStack);

            var totalBorder = new Border
            {
                Background = new SolidColorBrush(
                    Color.FromArgb(24, baseColor.R, baseColor.G, baseColor.B)
                ),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(12)
            };
            var totalStack = new StackPanel { Spacing = 4 };
            totalStack.Children.Add(new TextBlock
            {
                Text = "Total",
                FontSize = 11,
            });
            var totalText = new TextBlock
            {
                Text = CalculateTotal(editableItems),
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            };
            totalStack.Children.Add(totalText);
            totalBorder.Child = totalStack;
            Grid.SetColumn(totalBorder, 1);
            headerGrid.Children.Add(totalBorder);

            mainStack.Children.Add(headerGrid);
            mainStack.Children.Add(new Border
            {
                Height = 1,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                Opacity = 0.2,
            });

            // Customer Info
            var customerStack = new StackPanel { Spacing = 8 };
            customerStack.Children.Add(new TextBlock
            {
                Text = "Customer Information",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Opacity = 0.6
            });

            var customerGrid = new Grid { ColumnSpacing = 12 };
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            customerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var customerIcon = new FontIcon
            {
                Glyph = "\uE77B",
                FontSize = 16,
            };
            Grid.SetColumn(customerIcon, 0);
            customerGrid.Children.Add(customerIcon);

            var customerInfo = new StackPanel { Spacing = 2 };
            customerInfo.Children.Add(new TextBlock
            {
                Text = order.Customer?.Name ?? "N/A",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });
            customerInfo.Children.Add(new TextBlock
            {
                Text = order.Customer?.Phone ?? "N/A",
                FontSize = 12,
                Opacity = 0.6
            });
            Grid.SetColumn(customerInfo, 1);
            customerGrid.Children.Add(customerInfo);

            customerStack.Children.Add(customerGrid);
            mainStack.Children.Add(customerStack);
            mainStack.Children.Add(new Border
            {
                Height = 1,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                Opacity = 0.2,
            });

            // Order Items Section
            var itemsHeader = new Grid();
            itemsHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            itemsHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            itemsHeader.Children.Add(new TextBlock
            {
                Text = "Order Items",
                FontSize = 14,
                Opacity = 0.6,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Bottom
            });

            var addButton = new Button
            {
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new FontIcon { Glyph = "\uE710", FontSize = 10 },
                        new TextBlock { Text = "Add Product",                     
                                        FontSize = 12,
                                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                                        Opacity = 0.75
                                        }
                    }
                },
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addButton.Click += async (s, e) =>
            {
                await AddProductToOrder(editableItems, totalText);
            };
            Grid.SetColumn(addButton, 1);
            itemsHeader.Children.Add(addButton);

            mainStack.Children.Add(itemsHeader);

            // Table Header
            var tableHeader = new Grid
            {
                ColumnSpacing = 16,
                Padding = new Thickness(12),
                Background = new SolidColorBrush(
                    Color.FromArgb(24, baseColor.R, baseColor.G, baseColor.B)
                ),
                BorderBrush = GetResourceBrush("CardStrokeColorDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8, 8, 0, 0),
                Margin = new Thickness(0, 12, 0, 0),
                Opacity = 0.6
            };
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Pixel) });
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.1, GridUnitType.Star) });
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200, GridUnitType.Pixel) });
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180, GridUnitType.Pixel) });
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140, GridUnitType.Pixel) });
            tableHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Pixel) });

            var headers = new[]
            {
                ("SKU Code", 0, TextAlignment.Left),
                ("Product", 1, TextAlignment.Left),
                ("Unit Price", 2, TextAlignment.Right),
                ("Quantity", 3, TextAlignment.Center),
                ("Total", 4, TextAlignment.Right),
                ("", 5, TextAlignment.Center)
            };

            foreach (var (text, col, align) in headers)
            {
                var headerText = new TextBlock
                {
                    Text = text,
                    FontSize = 12,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    TextAlignment = align,
                };
                Grid.SetColumn(headerText, col);
                tableHeader.Children.Add(headerText);
            }
            mainStack.Children.Add(tableHeader);

            // Items List
            var itemsStack = new StackPanel { Spacing = 12 };

            foreach (var item in editableItems)
            {
                var itemControl = CreateEditableItemControl(item, editableItems, totalText);
                itemsStack.Children.Add(itemControl);
            }

            // Watch for collection changes
            editableItems.CollectionChanged += (s, e) =>
            {
                itemsStack.Children.Clear();
                foreach (var item in editableItems)
                {
                    var itemControl = CreateEditableItemControl(item, editableItems, totalText);
                    itemsStack.Children.Add(itemControl);
                }
                totalText.Text = CalculateTotal(editableItems);
            };

            mainStack.Children.Add(itemsStack);

            // Summary
            var summaryBorder = new Border
            {
                Background = GetResourceBrush("LayerFillColorDefaultBrush"),
                BorderBrush = GetResourceBrush("CardStrokeColorDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16)
            };

            scrollViewer.Content = mainStack;
            return scrollViewer;
        }
        
        ///
        private Border CreateEditableItemControl(
            EditableOrderItem item,
            ObservableCollection<EditableOrderItem> allItems,
            TextBlock totalText)
        {
            var baseBrush = GetResourceBrush("LayerFillColorDefaultBrush") as SolidColorBrush;
            var baseColor = baseBrush.Color;
            var itemBorder = new Border
            {
                Background = new SolidColorBrush(
                    Color.FromArgb(2, baseColor.R, baseColor.G, baseColor.B)
                ),
                BorderBrush = GetResourceBrush("CardStrokeColorDefaultBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid
            {
                ColumnSpacing = 16,
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90, GridUnitType.Pixel) }); // SKU
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Product
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160, GridUnitType.Pixel) }); // Unit Price
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180, GridUnitType.Pixel) }); // Quantity
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140, GridUnitType.Pixel) }); // Total
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Pixel) }); // Remove

            // SKU Code
            var skuText = new TextBlock
            {
                Text = item.ProductSku,
                FontSize = 13,
                Opacity = 0.6,
                VerticalAlignment = VerticalAlignment.Center,
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(item.ProductSku))
                    skuText.Text = item.ProductSku;
            };
            Grid.SetColumn(skuText, 0);
            grid.Children.Add(skuText);

            var productStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                VerticalAlignment = VerticalAlignment.Center
            };

            var productSearch = new AutoSuggestBox
            {
                Text = item.ProductName,
                PlaceholderText = "Search product...",
                Width = 400,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center
            };

            productSearch.TextChanged += async (s, e) =>
            {
                if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    var searchText = productSearch.Text;
                    if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length >= 2)
                    {
                        var products = await _orderService.SearchProductsAsync(searchText);
                        productSearch.ItemsSource = products;
                    }
                }
            };

            productSearch.SuggestionChosen += (s, e) =>
            {
                if (e.SelectedItem is Models.Product product)
                {
                    item.UpdateProduct(product);
                    productSearch.Text = product.Name;
                    totalText.Text = CalculateTotal(allItems);
                }
            };

            productSearch.ItemTemplate = CreateProductSuggestionTemplate();
            productStack.Children.Add(productSearch);

            var detailBtn = new Button
            {
                Width = 28,
                Height = 28,
                Padding = new Thickness(0),
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
                BorderThickness = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                Content = new FontIcon
                {
                    Glyph = "\uE946",
                    FontSize = 12,
                }
            };
            detailBtn.Click += async (s, e) =>
            {
                if (item.Product != null)
                {
                    await ShowProductDetailAsync(item.Product);
                }
            };
            productStack.Children.Add(detailBtn);

            Grid.SetColumn(productStack, 1);
            grid.Children.Add(productStack);

            // Unit Price
            var priceText = new TextBlock
            {
                Text = item.FormattedPrice,
                FontSize = 13,
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(item.FormattedPrice))
                    priceText.Text = item.FormattedPrice;
            };
            Grid.SetColumn(priceText, 2);
            grid.Children.Add(priceText);

            // Quantity Controls
            var qtyStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var decreaseBtn = new Button
            {
                Width = 32,
                Height = 32,
                Content = new FontIcon { Glyph = "\uE738", FontSize = 8 },
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent)

            };
            decreaseBtn.Click += (s, e) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    totalText.Text = CalculateTotal(allItems);
                }
            };
            qtyStack.Children.Add(decreaseBtn);

            var qtyBox = new TextBox
            {
                Text = item.Quantity.ToString(),
                FontSize = 13,
                Width = 60,
                TextAlignment = TextAlignment.Center,
                InputScope = new InputScope
                {
                    Names = { new InputScopeName(InputScopeNameValue.Number) }
                }
            };
            qtyBox.TextChanged += (s, e) =>
            {
                if (int.TryParse(qtyBox.Text, out int qty) && qty >= 1)
                {
                    item.Quantity = qty;
                    totalText.Text = CalculateTotal(allItems);
                }
            };
            qtyStack.Children.Add(qtyBox);

            var increaseBtn = new Button
            {
                Width = 32,
                Height = 32,
                Content = new FontIcon { Glyph = "\uE710", FontSize = 8 },
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
            };
            increaseBtn.Click += (s, e) =>
            {
                item.Quantity++;
                qtyBox.Text = item.Quantity.ToString();
                totalText.Text = CalculateTotal(allItems);
            };
            qtyStack.Children.Add(increaseBtn);

            Grid.SetColumn(qtyStack, 3);
            grid.Children.Add(qtyStack);

            var subtotalText = new TextBlock
            {
                Text = item.FormattedSubtotal,
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(item.FormattedSubtotal))
                {
                    subtotalText.Text = item.FormattedSubtotal;
                }
            };
            Grid.SetColumn(subtotalText, 4);
            grid.Children.Add(subtotalText);

            // Remove Button
            var removeBtn = new Button
            {
                Width = 36,
                Height = 36,
                Content = new FontIcon
                {
                    Glyph = "\uE74D",
                    FontSize = 12,
                    Foreground = GetResourceBrush("SystemFillColorCriticalBrush")
                },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            removeBtn.Click += (s, e) =>
            {
                allItems.Remove(item);
            };
            Grid.SetColumn(removeBtn, 5);
            grid.Children.Add(removeBtn);

            itemBorder.Child = grid;
            return itemBorder;
        }

        private DataTemplate CreateProductSuggestionTemplate()
        {
            return (DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(@"
<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>

    <Grid ColumnSpacing='12' Padding='8'>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width='50'/>
            <ColumnDefinition Width='*'/>
            <ColumnDefinition Width='Auto'/>
        </Grid.ColumnDefinitions>

        <Border Grid.Column='0'
                Width='50'
                Height='50'
                CornerRadius='4'
                Background='{ThemeResource SubtleFillColorSecondaryBrush}'>
            <Image Stretch='UniformToFill'
                   Source='{Binding Images[0]}'/>
        </Border>

        <StackPanel Grid.Column='1' VerticalAlignment='Center'>
            <TextBlock Text='{Binding Name}'
                       FontWeight='SemiBold'/>
            <TextBlock Text='{Binding Sku}'
                       FontSize='11'
                       Foreground='{ThemeResource TextFillColorSecondaryBrush}'/>
        </StackPanel>

        <TextBlock Grid.Column='2'
                   Text='{Binding FormattedPrice}'
                   VerticalAlignment='Center'
                   FontWeight='SemiBold'
                   Foreground='{ThemeResource AccentTextFillColorPrimaryBrush}'/>
    </Grid>
</DataTemplate>");
        }


        private async Task AddProductToOrder(ObservableCollection<EditableOrderItem> items, TextBlock totalText)
        {
            var newItem = new EditableOrderItem(
                new Models.Product { Name = "", Sku = "", ImportPrice = 0 },
                () => { }
            );
            newItem.Quantity = 1;
            items.Add(newItem);
        }

        private string CalculateTotal(ObservableCollection<EditableOrderItem> items)
        {
            var total = items.Sum(i => i.Subtotal);
            return $"{total:N0} ₫";
        }

        private async Task SaveOrderChangesAsync(Models.Order order, ObservableCollection<EditableOrderItem> items)
        {
            if (items.Count == 0)
            {
                await ShowErrorAsync("Order must have at least one item.");
                return;
            }

            try
            {
                IsLoading = true;

                Debug.WriteLine("Debug 1");

                var orderItems = items.Select(ei => new Models.OrderItem
                {
                    ProductId = ei.ProductId,
                    Quantity = ei.Quantity,
                    Price = ei.Price
                }).ToList();

                Debug.WriteLine("Debug 2");

                var updatedOrder = await _orderService.UpdateOrderAsync(order.Id, orderItems);

                Debug.WriteLine("Debug 3");

                if (updatedOrder != null)
                {
                    order.FinalPrice = updatedOrder.FinalPrice;
                    order.OrderItems = updatedOrder.OrderItems;
                    OnPropertyChanged(nameof(Orders));

                    await ShowSuccessAsync("Order updated successfully!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving order: {ex.Message}");
                await ShowErrorAsync($"Failed to update order: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchOrdersAsync()
        {
            try
            {
                Debug.WriteLine("Status: " + SelectedStatus);
                IsLoading = true;
                CurrentPage = 1; 

                var (ordersList, totalCount) = await _orderService.SearchOrdersAsync(
                    page: CurrentPage,
                    pageSize: PageSize,
                    fromDate: SearchFromDate,
                    toDate: SearchToDate,
                    customerId: SelectedCustomer?.Id,
                    productId: SelectedProduct?.Id,
                    minPrice: SearchMinPrice,
                    maxPrice: SearchMaxPrice,
                    status: SelectedStatus
                );

                Orders.Clear();
                foreach (var order in ordersList)
                {
                    Orders.Add(order);
                }

                TotalOrders = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching orders: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchFromDate = null;
            SearchToDate = null;
            SelectedCustomer = null;
            SelectedProduct = null;
            SearchMinPrice = double.NaN;
            SearchMaxPrice = double.NaN;
            SelectedStatus = null;

            await LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task PrintOrderAsync(Models.Order order)
        {
            if (XamlRoot == null) return;

            try
            {
                IsLoading = true;

                // Generate PDF
                var pdfBytes = _pdfService.GenerateOrderInvoice(order);
                var filePath = _pdfService.SavePdfToTemp(pdfBytes, order.Id);

                // Show preview dialog
                var dialog = new ContentDialog
                {
                    Title = $"Invoice - Order #{order.Id}",
                    PrimaryButtonText = "Open PDF",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = XamlRoot,
                    RequestedTheme = ElementTheme.Light,
                    Content = new StackPanel
                    {
                        Spacing = 16,
                        Children =
                    {
                        new TextBlock
                        {
                            Text = $"To open file for more details, click \"Open PDF\"!",
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = 14,
                            Opacity = 0.7
                        }
                    }
                    }
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Open PDF in default viewer
                    await Launcher.LaunchUriAsync(new Uri(filePath));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error printing order: {ex.Message}");
                await ShowErrorAsync($"Failed to generate PDF: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
