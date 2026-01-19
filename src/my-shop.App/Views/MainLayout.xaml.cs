using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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

            // Setup spotlight overlay events
            SpotlightOverlay.TourCompleted += OnTourCompleted;
            SpotlightOverlay.TourSkipped += OnTourSkipped;
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
                    "Customers" => typeof(CustomersScreenView),
                    "Vouchers" => typeof(VouchersScreenView),
                    "Reports" => typeof(ReportScreenView),
                    "Config" => typeof(ConfigScreenView),
                    _ => typeof(DashboardView)
                };

                ContentFrame.Navigate(pageType);
            }
        }

        // PUBLIC METHOD: Start the onboarding tour
        public async Task StartOnboardingTourAsync()
        {
            Debug.WriteLine("🔵 [MainLayout] Starting onboarding tour");

            // Wait for UI to be fully loaded
            await Task.Delay(500);

            var steps = new List<OnboardingStep>
            {
                new OnboardingStep
                {
                    Title = "Dashboard",
                    Description = "Your command center! View sales statistics, revenue, recent orders, and monitor your business performance at a glance.",
                    IconGlyph = "\uE80F",
                    TargetElement = DashboardNavItem
                },
                new OnboardingStep
                {
                    Title = "Products",
                    Description = "Manage your product catalog here. Add new products, update prices, upload images, organize by categories, and track inventory levels.",
                    IconGlyph = "\uE7B8",
                    TargetElement = ProductsNavItem
                },
                new OnboardingStep
                {
                    Title = "Orders",
                    Description = "Handle customer orders efficiently. Create new orders, update order status, print PDF invoices, and track payments all in one place.",
                    IconGlyph = "\uE7BF",
                    TargetElement = OrdersNavItem
                },
                new OnboardingStep
                {
                    Title = "Customers",
                    Description = "Build and manage your customer relationships. Store customer information, view purchase history, and track customer preferences.",
                    IconGlyph = "\uE716",
                    TargetElement = CustomersNavItem
                },
                new OnboardingStep
                {
                    Title = "Reports",
                    Description = "Gain insights from your data. Generate sales reports, analyze trends, track performance metrics, and make data-driven decisions.",
                    IconGlyph = "\uE9F9",
                    TargetElement = ReportsNavItem
                },
                new OnboardingStep
                {
                    Title = "Configuration",
                    Description = "Customize your shop settings. Configure system preferences, manage categories, set up payment methods, and adjust app behavior.",
                    IconGlyph = "\uE713",
                    TargetElement = ConfigNavItem
                },
                new OnboardingStep
                {
                    Title = "Settings",
                    Description = "Access your account settings here. Manage your license, view activation status, access help documentation, and sign out when needed.",
                    IconGlyph = "\uE713",
                    TargetElement = NavView.SettingsItem as FrameworkElement
                }
            };

            SpotlightOverlay.Visibility = Visibility.Visible;
            SpotlightOverlay.StartTour(steps);

            Debug.WriteLine($"✅ [MainLayout] Tour started with {steps.Count} steps");
        }

        private void OnTourCompleted(object? sender, EventArgs e)
        {
            Debug.WriteLine("✅ [MainLayout] Tour completed");
            SpotlightOverlay.Visibility = Visibility.Collapsed;
        }

        private void OnTourSkipped(object? sender, EventArgs e)
        {
            Debug.WriteLine("🔵 [MainLayout] Tour skipped by user");
            SpotlightOverlay.Visibility = Visibility.Collapsed;
        }
    }
}