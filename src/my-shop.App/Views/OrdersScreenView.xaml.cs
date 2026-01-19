using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Models;
using my_shop.App.ViewModels;
using System;
using System.Diagnostics;

namespace my_shop.App.Views
{
    public sealed partial class OrdersScreenView : Page
    {
        public OrdersScreenViewModel ViewModel { get; }

        public OrdersScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<OrdersScreenViewModel>();
            this.DataContext = ViewModel;
            Models.Order.SetViewModel(ViewModel);

            this.Loaded += async (s, e) =>
            {
                ViewModel.XamlRoot = this.XamlRoot;
                await ViewModel.CheckLicenseStatusAsync();
                await ViewModel.LoadOrdersCommand.ExecuteAsync(null);
            };

            ViewModel.ActivateLicenseRequested += () =>
            {
                this.Frame.Navigate(typeof(SettingsScreenView));
            };
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Order;
            if (order != null)
            {
                ViewModel.ToggleExpandCommand.Execute(order);
            }
        }

        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Order;
            if (order != null)
            {
                await ViewModel.MarkAsPaidCommand.ExecuteAsync(order);
            }
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Order;
            if (order != null)
            {
                await ViewModel.CancelOrderCommand.ExecuteAsync(order);
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Order;
            if (order != null)
            {
                await ViewModel.EditOrderCommand.ExecuteAsync(order);
            }
        }

        private async void CustomerSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var searchText = sender.Text;
                if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length >= 2)
                {
                    var customers = await ViewModel._orderService.SearchCustomersAsync(searchText);
                    sender.ItemsSource = customers;
                }
            }
        }

        private void CustomerSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is Customer customer)
            {
                ViewModel.SelectedCustomer = customer;
                sender.Text = customer.Name;
            }
        }

        private async void ProductSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var searchText = sender.Text;
                if (!string.IsNullOrWhiteSpace(searchText) && searchText.Length >= 2)
                {
                    var products = await ViewModel._orderService.SearchProductsAsync(searchText);
                    sender.ItemsSource = products;
                }
            }
        }

        private void ProductSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is Models.Product product)
            {
                ViewModel.SelectedProduct = product;
                sender.Text = product.Name;
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selected)
            {
                ViewModel.SelectedStatus = selected.Tag?.ToString();
                Debug.WriteLine($"Status selected: {ViewModel.SelectedStatus}");
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.Tag as Order;
            if (order != null)
            {
                await ViewModel.PrintOrderCommand.ExecuteAsync(order);
            }
        }

        public Visibility GetTrialBadgeVisibility(Models.Order order)
        {
            bool shouldShow = order.IsPaid && ViewModel.IsInTrial;
            System.Diagnostics.Debug.WriteLine($"[GetTrialBadge] Order #{order.Id}, IsPaid: {order.IsPaid}, IsInTrial: {ViewModel.IsInTrial}, Result: {shouldShow}");
            return shouldShow ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility GetExpiredBadgeVisibility(Models.Order order)
        {
            bool shouldShow = order.IsPaid && ViewModel.IsTrialExpired;
            System.Diagnostics.Debug.WriteLine($"[GetExpiredBadge] Order #{order.Id}, IsPaid: {order.IsPaid}, IsExpired: {ViewModel.IsTrialExpired}, Result: {shouldShow}");
            return shouldShow ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
