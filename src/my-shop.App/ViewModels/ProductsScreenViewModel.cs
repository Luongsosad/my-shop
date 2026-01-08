using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using my_shop.App.Models;
using my_shop.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class ProductsScreenViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<ProductModel> products = new();

        [ObservableProperty]
        private ObservableCollection<CategoryModel> categories = new();

        [ObservableProperty]
        private ObservableCollection<CustomerModel> customers = new();

        [ObservableProperty]
        private ObservableCollection<CartItemModel> cartItems = new();

        [ObservableProperty]
        private ProductModel? selectedProduct;

        [ObservableProperty]
        private CategoryModel? selectedCategory;

        partial void OnSelectedCategoryChanged(CategoryModel? value)
        {
            // Auto search when category changes
            _ = SearchProductsAsync();
        }

        [ObservableProperty]
        private CustomerModel? selectedCustomer;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isManagementMode = true;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private int cartTotalQuantity;

        [ObservableProperty]
        private int cartTotalPrice;

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private string customerPhone = string.Empty;

        public bool HasCartItems => CartTotalQuantity > 0;

        public ProductsScreenViewModel(IDataService dataService)
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
                var categoriesTask = _dataService.GetCategoriesAsync();
                var productsTask = _dataService.GetProductsAsync();
                var customersTask = _dataService.GetCustomersAsync();

                await Task.WhenAll(categoriesTask, productsTask, customersTask);

                Categories.Clear();
                Categories.Add(new CategoryModel { Id = 0, Name = "Tất cả" });
                foreach (var category in await categoriesTask)
                {
                    Categories.Add(category);
                }

                Products.Clear();
                foreach (var product in await productsTask)
                {
                    Products.Add(product);
                }

                Customers.Clear();
                foreach (var customer in await customersTask)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tải dữ liệu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SearchProductsAsync()
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var categoryId = SelectedCategory?.Id == 0 ? null : SelectedCategory?.Id;
                var searchTerm = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;

                var products = await _dataService.GetProductsAsync(searchTerm, categoryId);

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tìm kiếm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ToggleMode()
        {
            IsManagementMode = !IsManagementMode;
            
            // Clear cart when switching to management mode
            if (IsManagementMode)
            {
                ClearCart();
            }
        }

        [RelayCommand]
        public void AddToCart(ProductModel product)
        {
            if (product.Count <= 0)
            {
                ErrorMessage = "Sản phẩm đã hết hàng";
                HasError = true;
                return;
            }

            var existingItem = CartItems.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Count)
                {
                    existingItem.Quantity++;
                    UpdateCartTotals();
                }
                else
                {
                    ErrorMessage = "Không đủ hàng trong kho";
                    HasError = true;
                }
            }
            else
            {
                CartItems.Add(new CartItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.ImportPrice,
                    Quantity = 1,
                    ImageUrl = product.Images?.FirstOrDefault()
                });
                UpdateCartTotals();
            }
        }

        [RelayCommand]
        public void RemoveFromCart(CartItemModel item)
        {
            CartItems.Remove(item);
            UpdateCartTotals();
        }

        [RelayCommand]
        public void IncreaseQuantity(CartItemModel item)
        {
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null && item.Quantity < product.Count)
            {
                item.Quantity++;
                UpdateCartTotals();
            }
            else
            {
                ErrorMessage = "Không đủ hàng trong kho";
                HasError = true;
            }
        }

        [RelayCommand]
        public void DecreaseQuantity(CartItemModel item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
                UpdateCartTotals();
            }
            else
            {
                RemoveFromCart(item);
            }
        }

        [RelayCommand]
        public void ClearCart()
        {
            CartItems.Clear();
            SelectedCustomer = null;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
            UpdateCartTotals();
        }

        [RelayCommand]
        public async Task CreateOrderAsync()
        {
            if (CartItems.Count == 0)
            {
                ErrorMessage = "Giỏ hàng trống";
                HasError = true;
                return;
            }

            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 Creating order with {CartItems.Count} items");
                
                var orderModel = new CreateOrderModel
                {
                    CustomerId = SelectedCustomer?.Id,
                    CustomerName = string.IsNullOrWhiteSpace(CustomerName) ? null : CustomerName,
                    CustomerPhone = string.IsNullOrWhiteSpace(CustomerPhone) ? null : CustomerPhone,
                    Items = CartItems.Select(item => new OrderItemModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList(),
                    FinalPrice = CartTotalPrice
                };

                var order = await _dataService.CreateOrderAsync(orderModel);
                
                System.Diagnostics.Debug.WriteLine($"✅ Order created: #{order.Id}");
                
                // Clear cart IMMEDIATELY after successful order
                System.Diagnostics.Debug.WriteLine("🔵 Clearing cart...");
                ClearCart();
                System.Diagnostics.Debug.WriteLine($"✅ Cart cleared. Items: {CartItems.Count}");
                
                // Reload products to update stock
                System.Diagnostics.Debug.WriteLine("🔵 Reloading products...");
                await LoadDataAsync();
                System.Diagnostics.Debug.WriteLine("✅ Products reloaded");

                ErrorMessage = $"Đơn hàng #{order.Id} đã được tạo thành công!";
                HasError = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Create order error: {ex.Message}");
                HasError = true;
                ErrorMessage = $"Không thể tạo đơn hàng: {ex.Message}";
                throw; // Re-throw để code-behind có thể catch
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void EditProduct(ProductModel product)
        {
            SelectedProduct = product;
            // Dialog will be opened by the view
        }

        [RelayCommand]
        public void AddNewProduct()
        {
            SelectedProduct = null;
            // Dialog will be opened by the view
        }

        [RelayCommand]
        public async Task CreateProductAsync(CreateProductModel model)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var newProduct = await _dataService.CreateProductAsync(model);
                Products.Add(newProduct);
                
                ErrorMessage = $"Đã thêm sản phẩm '{newProduct.Name}' thành công!";
                HasError = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tạo sản phẩm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task UpdateProductAsync(UpdateProductModel model)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 Updating product: {model.Name}");
                System.Diagnostics.Debug.WriteLine($"🔵 Images count: {model.Images?.Count ?? 0}");
                if (model.Images != null && model.Images.Count > 0)
                {
                    foreach (var img in model.Images)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔵 Image URL: {img}");
                    }
                }
                
                var updatedProduct = await _dataService.UpdateProductAsync(model);
                
                System.Diagnostics.Debug.WriteLine($"✅ Product updated successfully. Images in response: {updatedProduct.Images?.Count ?? 0}");
                
                // Update in list
                var existingProduct = Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (existingProduct != null)
                {
                    var index = Products.IndexOf(existingProduct);
                    Products[index] = updatedProduct;
                }
                
                ErrorMessage = $"Đã cập nhật sản phẩm '{updatedProduct.Name}' thành công!";
                HasError = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Update product error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                HasError = true;
                ErrorMessage = $"Không thể cập nhật sản phẩm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task DeleteProductAsync(ProductModel product)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var result = await _dataService.DeleteProductAsync(product.Id);
                if (result)
                {
                    Products.Remove(product);
                }
                else
                {
                    ErrorMessage = "Không thể xóa sản phẩm";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Lỗi khi xóa sản phẩm: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCartTotals()
        {
            CartTotalQuantity = CartItems.Sum(i => i.Quantity);
            CartTotalPrice = CartItems.Sum(i => i.Quantity * i.Price);
            OnPropertyChanged(nameof(HasCartItems));
        }
    }
}
