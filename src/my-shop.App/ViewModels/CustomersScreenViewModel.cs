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
    public partial class CustomersScreenViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        [ObservableProperty]
        private ObservableCollection<CustomerModel> customers = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 10;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private int totalCustomers;

        public bool HasNoCustomers => !IsLoading && Customers.Count == 0;

        public CustomersScreenViewModel(IDataService dataService)
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
                var (customers, totalCount) = await _dataService.GetCustomersWithPagingAsync(
                    page: CurrentPage,
                    pageSize: PageSize
                );

                Customers.Clear();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }

                TotalCustomers = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
                OnPropertyChanged(nameof(HasNoCustomers));
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
        public async Task SearchCustomersAsync()
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                CurrentPage = 1; // Reset to first page on new search
                
                var searchTerm = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;
                var (customers, totalCount) = await _dataService.GetCustomersWithPagingAsync(
                    page: CurrentPage,
                    pageSize: PageSize,
                    searchTerm: searchTerm
                );

                Customers.Clear();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }

                TotalCustomers = totalCount;
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
                OnPropertyChanged(nameof(HasNoCustomers));
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
        public async Task ClearSearchAsync()
        {
            SearchText = string.Empty;
            await LoadDataAsync();
        }

        [RelayCommand]
        public async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                if (string.IsNullOrWhiteSpace(SearchText))
                    await LoadDataAsync();
                else
                    await SearchCustomersAsync();
            }
        }

        [RelayCommand]
        public async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                if (string.IsNullOrWhiteSpace(SearchText))
                    await LoadDataAsync();
                else
                    await SearchCustomersAsync();
            }
        }

        [RelayCommand]
        public async Task CreateCustomerAsync((string Name, string Phone) input)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var newCustomer = await _dataService.CreateCustomerAsync(input.Name, input.Phone);
                Customers.Add(newCustomer);
                OnPropertyChanged(nameof(HasNoCustomers));
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể tạo khách hàng: {ex.Message}";
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task UpdateCustomerAsync((int Id, string Name, string Phone) input)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var updatedCustomer = await _dataService.UpdateCustomerAsync(input.Id, input.Name, input.Phone);
                
                var existingCustomer = Customers.FirstOrDefault(c => c.Id == input.Id);
                if (existingCustomer != null)
                {
                    var index = Customers.IndexOf(existingCustomer);
                    Customers[index] = updatedCustomer;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Không thể cập nhật khách hàng: {ex.Message}";
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task DeleteCustomerAsync(int id)
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var result = await _dataService.DeleteCustomerAsync(id);
                if (result)
                {
                    var customer = Customers.FirstOrDefault(c => c.Id == id);
                    if (customer != null)
                    {
                        Customers.Remove(customer);
                        OnPropertyChanged(nameof(HasNoCustomers));
                    }
                }
                else
                {
                    ErrorMessage = "Không thể xóa khách hàng";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Lỗi khi xóa khách hàng: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
