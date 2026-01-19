using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using my_shop.App.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

namespace my_shop.App.Views
{
    public sealed partial class ProductsScreenView : Page
    {
        public ProductsScreenViewModel ViewModel { get; }

        public ProductsScreenView()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: Constructor START");
                
                // CRITICAL: Get ViewModel BEFORE InitializeComponent
                ViewModel = App.GetService<ProductsScreenViewModel>();
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: ViewModel created");
                
                this.InitializeComponent();
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: InitializeComponent DONE");
                
                this.DataContext = ViewModel;
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: DataContext set");
                
                // Subscribe to property changes
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: Constructor END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ProductsScreenView Constructor Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsManagementMode))
            {
                UpdateModeUI();
            }
        }

        private void UpdateModeUI()
        {
            // Update toggle button text and icon
            if (ToggleModeText != null && ToggleModeIcon != null)
            {
                if (ViewModel.IsManagementMode)
                {
                    ToggleModeText.Text = "Chuyển sang bán hàng";
                    ToggleModeIcon.Glyph = "\uE7BF"; // Cart icon
                }
                else
                {
                    ToggleModeText.Text = "Chuyển sang quản lý";
                    ToggleModeIcon.Glyph = "\uE74E"; // Settings icon
                }
            }
            
            // TEMPORARILY DISABLED - causes WinRT exception
            // Bindings.Update();
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                ViewModel.SearchSortBy = tag;
            }
        }

        private void SortOrderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                ViewModel.SearchSortOrder = tag;
            }
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: Page_Loaded START");
                await ViewModel.LoadDataCommand.ExecuteAsync(null);
                System.Diagnostics.Debug.WriteLine("🔵 ProductsScreenView: Page_Loaded END");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Page_Loaded Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            }
        }

        private void SearchBox_KeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            try
            {
                // Use InitiateSearch which resets to page 1
                ViewModel.InitiateSearchCommand.Execute(null);
                args.Handled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SearchBox_KeyboardAcceleratorInvoked Error: {ex.Message}");
            }
        }

        private void ToggleModeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 Toggle Mode: {ViewModel.IsManagementMode} -> {!ViewModel.IsManagementMode}");
                System.Diagnostics.Debug.WriteLine($"🔵 Cart items count: {ViewModel.CartItems.Count}");
                
                ViewModel.ToggleModeCommand.Execute(null);
                
                System.Diagnostics.Debug.WriteLine($"✅ Toggle mode command executed");
                
                // Defer binding update to avoid WinRT exception
                _ = this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    try
                    {
                        Bindings.Update();
                        System.Diagnostics.Debug.WriteLine($"✅ Bindings updated");
                    }
                    catch (Exception ex2)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Bindings.Update Error: {ex2.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ToggleModeButton_Click Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private async void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Microsoft.UI.Xaml.Controls.Button;
            var product = button?.CommandParameter as my_shop.App.Models.ProductModel;
            
            if (product != null)
            {
                await ShowEditProductDialog(product);
            }
        }

        private async void AddNewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await ShowEditProductDialog(null);
        }

        private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Microsoft.UI.Xaml.Controls.Button;
            var product = button?.CommandParameter as my_shop.App.Models.ProductModel;
            
            if (product != null)
            {
                await ShowDeleteConfirmationDialog(product);
            }
        }

        private async System.Threading.Tasks.Task ShowEditProductDialog(my_shop.App.Models.ProductModel? product)
        {
            // Create dialog
            var dialog = new Dialogs.ProductEditDialog
            {
                XamlRoot = this.XamlRoot
            };
            
            // Load product data and categories
            dialog.LoadProduct(product, ViewModel.Categories.ToList());

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                // Validate
                if (!dialog.Validate(out string errorMessage))
                {
                    // Show validation error
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi nhập liệu",
                        Content = errorMessage,
                        CloseButtonText = "Đóng",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                    
                    // Reopen dialog
                    await ShowEditProductDialog(product);
                    return;
                }

                try
                {
                    if (product == null)
                    {
                        // Create new product
                        var createModel = dialog.GetCreateModel();
                        await ViewModel.CreateProductCommand.ExecuteAsync(createModel);
                        System.Diagnostics.Debug.WriteLine($"🔵 Created new product: {createModel.Name}");
                    }
                    else
                    {
                        // Update existing product
                        var updateModel = dialog.GetUpdateModel();
                        await ViewModel.UpdateProductCommand.ExecuteAsync(updateModel);
                        System.Diagnostics.Debug.WriteLine($"🔵 Updated product: {updateModel.Name}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Save product error: {ex.Message}");
                    
                    // Show error dialog
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = $"Không thể lưu sản phẩm:\n\n{ex.Message}",
                        CloseButtonText = "Đóng",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async System.Threading.Tasks.Task ShowDeleteConfirmationDialog(my_shop.App.Models.ProductModel product)
        {
            var dialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc chắn muốn xóa sản phẩm '{product.Name}'?\n\nHành động này không thể hoàn tác.",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteProductCommand.ExecuteAsync(product);
            }
        }

        private void CustomerTypeRadio_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (CustomerTypeRadio?.SelectedIndex == 0)
            {
                // Existing customer
                if (CustomerComboBox != null)
                    CustomerComboBox.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                if (NewCustomerPanel != null)
                    NewCustomerPanel.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                // New customer
                if (CustomerComboBox != null)
                    CustomerComboBox.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                if (NewCustomerPanel != null)
                    NewCustomerPanel.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            }
        }

        private async void CreateOrderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 CreateOrderButton_Click START");
                
                // Validate cart
                if (ViewModel.CartItems.Count == 0)
                {
                    await ShowErrorDialog("Giỏ hàng trống", "Vui lòng thêm sản phẩm vào giỏ hàng trước khi tạo đơn.");
                    return;
                }

                // Validate customer selection
                if (CustomerTypeRadio?.SelectedIndex == 0)
                {
                    // Existing customer
                    if (ViewModel.SelectedCustomer == null)
                    {
                        await ShowErrorDialog("Chưa chọn khách hàng", "Vui lòng chọn khách hàng hoặc tạo mới.");
                        return;
                    }
                }
                else
                {
                    // New customer
                    if (string.IsNullOrWhiteSpace(ViewModel.CustomerName))
                    {
                        await ShowErrorDialog("Thiếu thông tin", "Vui lòng nhập tên khách hàng.");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(ViewModel.CustomerPhone))
                    {
                        await ShowErrorDialog("Thiếu thông tin", "Vui lòng nhập số điện thoại khách hàng.");
                        return;
                    }
                }

                // Create order - WAIT for completion
                await ViewModel.CreateOrderCommand.ExecuteAsync(null);
                
                System.Diagnostics.Debug.WriteLine("🔵 Order created successfully");
                System.Diagnostics.Debug.WriteLine($"🔵 Cart items after order: {ViewModel.CartItems.Count}");
                
                // Force UI update
                Bindings.Update();
                
                // Close flyout AFTER cart is cleared
                if (CartFlyout != null)
                {
                    CartFlyout.Hide();
                }

                // Show success dialog
                await ShowSuccessDialog("Thành công", "Đơn hàng đã được tạo thành công!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateOrderButton_Click Error: {ex.Message}");
                await ShowErrorDialog("Lỗi tạo đơn hàng", $"Không thể tạo đơn hàng:\n\n{ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task ShowErrorDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "Đóng",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task ShowSuccessDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        // Cart Item Button Handlers
        private void IncreaseQuantityButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var button = sender as Microsoft.UI.Xaml.Controls.Button;
                // Get item from DataContext (set automatically by DataTemplate)
                var item = button?.DataContext as my_shop.App.Models.CartItemModel;
                if (item != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 IncreaseQuantityButton_Click for {item.ProductName}");
                    ViewModel.IncreaseQuantityCommand.Execute(item);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ IncreaseQuantityButton_Click: item is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ IncreaseQuantityButton_Click Error: {ex.Message}");
            }
        }

        private void DecreaseQuantityButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var button = sender as Microsoft.UI.Xaml.Controls.Button;
                // Get item from DataContext (set automatically by DataTemplate)
                var item = button?.DataContext as my_shop.App.Models.CartItemModel;
                if (item != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 DecreaseQuantityButton_Click for {item.ProductName}");
                    ViewModel.DecreaseQuantityCommand.Execute(item);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ DecreaseQuantityButton_Click: item is null");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DecreaseQuantityButton_Click Error: {ex.Message}");
            }
        }

        private void RemoveFromCartButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var button = sender as Microsoft.UI.Xaml.Controls.Button;
                // Get item from DataContext (set automatically by DataTemplate)
                var item = button?.DataContext as my_shop.App.Models.CartItemModel;
                if (item != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 RemoveFromCartButton_Click for {item.ProductName}");
                    ViewModel.RemoveFromCartCommand.Execute(item);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ RemoveFromCartButton_Click: item is null");
                }            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RemoveFromCartButton_Click Error: {ex.Message}");
            }
        }
    }
}
