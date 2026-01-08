using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using my_shop.App.ViewModels;
using System;

namespace my_shop.App.Views
{
    public sealed partial class CustomersScreenView : Page
    {
        public CustomersScreenViewModel ViewModel { get; }

        public CustomersScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<CustomersScreenViewModel>();
            this.DataContext = ViewModel;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync();
        }

        private void SearchBox_KeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            _ = ViewModel.SearchCustomersAsync();
            args.Handled = true;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ViewModel.SearchCustomersAsync();
        }

        private async void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowCustomerDialog(null);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button?.CommandParameter as my_shop.App.Models.CustomerModel;
            
            if (customer != null)
            {
                await ShowCustomerDialog(customer);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var customer = button?.CommandParameter as my_shop.App.Models.CustomerModel;
            
            if (customer != null)
            {
                await ShowDeleteConfirmationDialog(customer);
            }
        }

        private async System.Threading.Tasks.Task ShowCustomerDialog(my_shop.App.Models.CustomerModel? customer)
        {
            var dialog = new ContentDialog
            {
                Title = customer == null ? "Thêm khách hàng mới" : $"Sửa khách hàng: {customer.Name}",
                PrimaryButtonText = "Lưu",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            // Create input form
            var panel = new StackPanel { Spacing = 12 };
            
            var nameBox = new TextBox 
            { 
                Header = "Tên khách hàng *",
                PlaceholderText = "Nhập tên khách hàng",
                Text = customer?.Name ?? ""
            };
            
            var phoneBox = new TextBox 
            { 
                Header = "Số điện thoại *",
                PlaceholderText = "Nhập số điện thoại",
                Text = customer?.Phone ?? ""
            };

            panel.Children.Add(nameBox);
            panel.Children.Add(phoneBox);
            
            dialog.Content = panel;

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                // Validate
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    await ShowErrorDialog("Lỗi", "Tên khách hàng không được để trống");
                    await ShowCustomerDialog(customer);
                    return;
                }

                if (string.IsNullOrWhiteSpace(phoneBox.Text))
                {
                    await ShowErrorDialog("Lỗi", "Số điện thoại không được để trống");
                    await ShowCustomerDialog(customer);
                    return;
                }

                try
                {
                    if (customer == null)
                    {
                        // Create
                        await ViewModel.CreateCustomerAsync((nameBox.Text, phoneBox.Text));
                    }
                    else
                    {
                        // Update
                        await ViewModel.UpdateCustomerAsync((customer.Id, nameBox.Text, phoneBox.Text));
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorDialog("Lỗi", $"Không thể lưu khách hàng:\n\n{ex.Message}");
                }
            }
        }

        private async System.Threading.Tasks.Task ShowDeleteConfirmationDialog(my_shop.App.Models.CustomerModel customer)
        {
            var dialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc chắn muốn xóa khách hàng '{customer.Name}'?\n\nHành động này không thể hoàn tác.",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteCustomerAsync(customer.Id);
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
    }
}
