using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using my_shop.App.Models;
using my_shop.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class ProductsScreenViewModel : ObservableObject
    {
        private readonly IDataService _dataService;
        private readonly VoucherService _voucherService;

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
            // Auto search when category changes, reset to page 1
            CurrentPage = 1;
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
        private int cartOriginalPrice;

        [ObservableProperty]
        private int cartDiscountAmount;

        [ObservableProperty]
        private string voucherCode = string.Empty;

        [ObservableProperty]
        private VoucherDiscountResult? appliedVoucherResult;

        [ObservableProperty]
        private string? voucherMessage;

        [ObservableProperty]
        private bool hasVoucherError;

        public bool HasVoucherApplied => AppliedVoucherResult != null && AppliedVoucherResult.Success;

        [ObservableProperty]
        private string customerName = string.Empty;

        [ObservableProperty]
        private string customerPhone = string.Empty;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 10;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private int totalProducts;

        // Advanced Search Properties
        [ObservableProperty]
        private bool isSearchExpanded = false;

        [ObservableProperty]
        private double searchMinPrice = double.NaN;

        [ObservableProperty]
        private double searchMaxPrice = double.NaN;

        [ObservableProperty]
        private double searchMinStock = double.NaN;

        [ObservableProperty]
        private double searchMaxStock = double.NaN;

        [ObservableProperty]
        private string? searchSortBy = "name"; // name, price, stock

        [ObservableProperty]
        private string? searchSortOrder = "asc"; // asc, desc

        public bool HasCartItems => CartTotalQuantity > 0;

        public ProductsScreenViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _voucherService = new VoucherService();
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
                var customersTask = _dataService.GetCustomersAsync();

                await Task.WhenAll(categoriesTask, customersTask);

                Categories.Clear();
                Categories.Add(new CategoryModel { Id = 0, Name = "All" });
                foreach (var category in await categoriesTask)
                {
                    Categories.Add(category);
                }

                Customers.Clear();
                foreach (var customer in await customersTask)
                {
                    Customers.Add(customer);
                }

                // Load products with paging
                CurrentPage = 1;
                await SearchProductsAsync();
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Unable to load data: {ex.Message}";
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
                // DON'T reset CurrentPage here - let caller decide
                // CurrentPage = 1; // ← REMOVED
                
                var categoryId = SelectedCategory?.Id == 0 ? null : SelectedCategory?.Id;
                var searchTerm = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;

                // Get all products first with basic filters
                var allProducts = await _dataService.GetProductsAsync(searchTerm, categoryId);

                // Apply advanced filters client-side
                var filtered = allProducts.AsEnumerable();

                // Price range filter
                if (!double.IsNaN(SearchMinPrice) && SearchMinPrice > 0)
                {
                    filtered = filtered.Where(p => p.ImportPrice >= (int)SearchMinPrice);
                }
                if (!double.IsNaN(SearchMaxPrice) && SearchMaxPrice > 0)
                {
                    filtered = filtered.Where(p => p.ImportPrice <= (int)SearchMaxPrice);
                }

                // Stock range filter
                if (!double.IsNaN(SearchMinStock) && SearchMinStock > 0)
                {
                    filtered = filtered.Where(p => p.Count >= (int)SearchMinStock);
                }
                if (!double.IsNaN(SearchMaxStock) && SearchMaxStock > 0)
                {
                    filtered = filtered.Where(p => p.Count <= (int)SearchMaxStock);
                }

                // Sorting
                filtered = (SearchSortBy, SearchSortOrder) switch
                {
                    ("price", "asc") => filtered.OrderBy(p => p.ImportPrice),
                    ("price", "desc") => filtered.OrderByDescending(p => p.ImportPrice),
                    ("stock", "asc") => filtered.OrderBy(p => p.Count),
                    ("stock", "desc") => filtered.OrderByDescending(p => p.Count),
                    ("name", "desc") => filtered.OrderByDescending(p => p.Name),
                    _ => filtered.OrderBy(p => p.Name) // default: name asc
                };

                var filteredList = filtered.ToList();
                
                // Apply paging
                var totalCount = filteredList.Count;
                var skip = (CurrentPage - 1) * PageSize;
                var products = filteredList.Skip(skip).Take(PageSize).ToList();

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }

                TotalProducts = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Cannot search: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Helper command for basic search button (resets to page 1)
        [RelayCommand]
        public async Task InitiateSearchAsync()
        {
            CurrentPage = 1; // Reset to page 1 for new search
            await SearchProductsAsync();
        }

        [RelayCommand]
        public void ToggleSearch()
        {
            IsSearchExpanded = !IsSearchExpanded;
        }

        [RelayCommand]
        public async Task ClearSearchAsync()
        {
            SearchText = string.Empty;
            SelectedCategory = categories.FirstOrDefault(); // Select "Tất cả"
            SearchMinPrice = double.NaN;
            SearchMaxPrice = double.NaN;
            SearchMinStock = double.NaN;
            SearchMaxStock = double.NaN;
            SearchSortBy = "name";
            SearchSortOrder = "asc";
            CurrentPage = 1; // Reset to page 1
            await SearchProductsAsync();
        }

        [RelayCommand]
        public async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await SearchProductsAsync(); // Will use new CurrentPage
            }
        }

        [RelayCommand]
        public async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await SearchProductsAsync(); // Will use new CurrentPage
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
                ErrorMessage = "The product is out of stock";
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
                    ErrorMessage = "Insufficient stock";
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
            System.Diagnostics.Debug.WriteLine($"🔵 RemoveFromCart called for {item.ProductName}");
            CartItems.Remove(item);
            System.Diagnostics.Debug.WriteLine($"✅ Removed. Cart now has {CartItems.Count} items");
            UpdateCartTotals();
        }

        [RelayCommand]
        public void IncreaseQuantity(CartItemModel item)
        {
            System.Diagnostics.Debug.WriteLine($"🔵 IncreaseQuantity called for {item.ProductName}");
            
            var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null && item.Quantity < product.Count)
            {
                item.Quantity++;
                System.Diagnostics.Debug.WriteLine($"✅ Increased to {item.Quantity}");
                UpdateCartTotals();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Cannot increase - max stock reached");
                ErrorMessage = "Insufficient stock";
                HasError = true;
            }
        }

        [RelayCommand]
        public void DecreaseQuantity(CartItemModel item)
        {
            System.Diagnostics.Debug.WriteLine($"🔵 DecreaseQuantity called for {item.ProductName}");
            
            if (item.Quantity > 1)
            {
                item.Quantity--;
                System.Diagnostics.Debug.WriteLine($"✅ Decreased to {item.Quantity}");
                UpdateCartTotals();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"✅ Removing item (quantity was 1)");
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
            VoucherCode = string.Empty;
            AppliedVoucherResult = null;
            VoucherMessage = null;
            HasVoucherError = false;
            UpdateCartTotals();
        }

        [RelayCommand]
        public async Task ApplyVoucherAsync()
        {
            if (string.IsNullOrWhiteSpace(VoucherCode))
            {
                VoucherMessage = "Please enter the voucher code";
                HasVoucherError = true;
                return;
            }

            if (CartTotalQuantity == 0)
            {
                VoucherMessage = "Cart is empty";
                HasVoucherError = true;
                return;
            }

            try
            {
                var orderAmount = CartItems.Sum(i => i.Quantity * i.Price);
                var result = await _voucherService.CheckVoucherAsync(VoucherCode.Trim().ToUpper(), orderAmount);

                if (result != null && result.Success)
                {
                    AppliedVoucherResult = result;
                    UpdateCartTotals();
                    VoucherMessage = result.Message ?? "Voucher applied!";
                    HasVoucherError = false;
                }
                else
                {
                    AppliedVoucherResult = null;
                    VoucherMessage = result?.Message ?? "The voucher is invalid or has expired";
                    HasVoucherError = true;
                    UpdateCartTotals();
                }
            }
            catch (Exception ex)
            {
                AppliedVoucherResult = null;
                VoucherMessage = $"Erorr: {ex.Message}";
                HasVoucherError = true;
                UpdateCartTotals();
            }
        }

        [RelayCommand]
        public void RemoveVoucher()
        {
            AppliedVoucherResult = null;
            VoucherCode = string.Empty;
            VoucherMessage = null;
            HasVoucherError = false;
            UpdateCartTotals();
        }

        [RelayCommand]
        public async Task CreateOrderAsync()
        {
            if (CartItems.Count == 0)
            {
                ErrorMessage = "Cart is empty";
                HasError = true;
                return;
            }

            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
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
                    FinalPrice = CartTotalPrice,
                    VoucherCode = AppliedVoucherResult != null && AppliedVoucherResult.Success ? VoucherCode.Trim().ToUpper() : null
                };

                var order = await _dataService.CreateOrderAsync(orderModel);
                
                // Clear cart IMMEDIATELY after successful order
                ClearCart();
                
                // Reload products to update stock
                await LoadDataAsync();

                var voucherInfo = AppliedVoucherResult != null && AppliedVoucherResult.Success ? $" (Discount: {CartDiscountAmount:N0}đ)" : "";
                ErrorMessage = $"Order #{order.Id} has been successfully created!{voucherInfo}";
                HasError = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Unable to create order: {ex.Message}";
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
                
                ErrorMessage = $"Added product '{newProduct.Name}' succesfully!";
                HasError = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Unable to create product: {ex.Message}";
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
                var updatedProduct = await _dataService.UpdateProductAsync(model);
                
                // Update in list
                var existingProduct = Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
                if (existingProduct != null)
                {
                    var index = Products.IndexOf(existingProduct);
                    Products[index] = updatedProduct;
                }
                
                ErrorMessage = $"Updated product '{updatedProduct.Name}' successfully!";
                HasError = false;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Unable to update products: {ex.Message}";
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
                    ErrorMessage = "The product cannot be deleted";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error when deleting a product: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCartTotals()
        {
            CartTotalQuantity = CartItems.Sum(i => i.Quantity);
            CartOriginalPrice = CartItems.Sum(i => i.Quantity * i.Price);
            
            // Calculate discount if voucher is applied
            if (AppliedVoucherResult != null && AppliedVoucherResult.Success)
            {
                CartDiscountAmount = AppliedVoucherResult.DiscountAmount;
                CartTotalPrice = AppliedVoucherResult.FinalAmount;
            }
            else
            {
                CartDiscountAmount = 0;
                CartTotalPrice = CartOriginalPrice;
            }
            
            OnPropertyChanged(nameof(HasCartItems));
            OnPropertyChanged(nameof(HasVoucherApplied));
        }
    }
}
