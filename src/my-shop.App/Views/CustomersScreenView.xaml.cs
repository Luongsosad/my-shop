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
                Title = customer == null ? "Add new customer" : $"Update customer: {customer.Name}",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                RequestedTheme = ElementTheme.Light,
                XamlRoot = this.XamlRoot
            };

            // Create input form
            var panel = new StackPanel { Spacing = 12 };
            
            var nameBox = new TextBox 
            { 
                Header = "Customer's name",
                PlaceholderText = "Enter customer name",
                Text = customer?.Name ?? ""
            };
            
            var phoneBox = new TextBox 
            { 
                Header = "Phone number",
                PlaceholderText = "Enter phone number",
                Text = customer?.Phone ?? ""
            };

            panel.Children.Add(nameBox);
            panel.Children.Add(phoneBox);

            var contentRoot = new Border
            {
                Width = 420,              
                CornerRadius = new CornerRadius(8),
                Child = panel
            };

            dialog.Content = contentRoot;

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                // Validate
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    await ShowErrorDialog("Error", "Customer name cannot be left blank");
                    await ShowCustomerDialog(customer);
                    return;
                }

                if (string.IsNullOrWhiteSpace(phoneBox.Text))
                {
                    await ShowErrorDialog("Error", "The phone number must not be left blank");
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
                    await ShowErrorDialog("Error", $"Failed to save customer:\n\n{ex.Message}");
                }
            }
        }

        private async System.Threading.Tasks.Task ShowDeleteConfirmationDialog(my_shop.App.Models.CustomerModel customer)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirm deletion",
                Content = $"Are you sure you want to delete customer '{customer.Name}'? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                RequestedTheme = ElementTheme.Light,
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
                CloseButtonText = "Close",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
