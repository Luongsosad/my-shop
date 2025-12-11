using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace my_shop.App.Views
{
    public sealed partial class MainLayout : Page
    {
        public MainLayout()
        {
            this.InitializeComponent();
            
            // Navigate to Dashboard by default
            ContentFrame.Navigate(typeof(DashboardView));
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsScreenView));
            }
            else if (args.SelectedItemContainer is NavigationViewItem item)
            {
                string tag = item.Tag?.ToString() ?? "";
                
                Type pageType = tag switch
                {
                    "Dashboard" => typeof(DashboardView),
                    "Products" => typeof(ProductsScreenView),
                    "Orders" => typeof(OrdersScreenView),
                    "Reports" => typeof(ReportScreenView),
                    "Config" => typeof(ConfigScreenView),
                    _ => typeof(DashboardView)
                };
                
                ContentFrame.Navigate(pageType);
            }
        }
    }
}
