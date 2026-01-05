using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using my_shop.App.Services;
using my_shop.App.ViewModels;
using my_shop.App.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace my_shop.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Microsoft.UI.Xaml.Application
    {
        private Window? _window;
        public IHost AppHost { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            try
            {
                InitializeComponent();
                
                System.Diagnostics.Debug.WriteLine("🔵 App: InitializeComponent completed");
                
                // Load .env file from root directory
                var envPath = System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".env");
                System.Diagnostics.Debug.WriteLine($"🔵 App: Looking for .env at: {envPath}");
                
                if (System.IO.File.Exists(envPath))
                {
                    System.Diagnostics.Debug.WriteLine("🔵 App: .env file found, loading...");
                    foreach (var line in System.IO.File.ReadAllLines(envPath))
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
                        var parts = line.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                            System.Diagnostics.Debug.WriteLine($"🔵 App: Set env var: {parts[0].Trim()}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ App: .env file not found");
                }
                
                // Add unhandled exception handler for debugging
                this.UnhandledException += App_UnhandledException;

                System.Diagnostics.Debug.WriteLine("🔵 App: Configuring services...");
                
                AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
                {
                    services.AddTransient<MainWindow>();
                    services.AddTransient<MainLayout>();

                    // Add View Services
                    services.AddTransient<ConfigScreenView>();
                    services.AddTransient<CustomersScreenView>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<LoginScreenView>();
                    services.AddTransient<OrdersScreenView>();
                    services.AddTransient<ProductsScreenView>();
                    services.AddTransient<ReportScreenView>();
                    services.AddTransient<SettingsScreenView>();

                    // Add ViewModel Services
                    services.AddTransient<ConfigScreenViewModel>();
                    services.AddTransient<CustomersScreenViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<LoginScreenViewModel>();
                    services.AddTransient<OrdersScreenViewModel>();
                    services.AddTransient<ProductsScreenViewModel>();
                    services.AddTransient<ReportScreenViewModel>();
                    services.AddTransient<SettingsScreenViewModel>();

                    // Add Services
                    services.AddSingleton<IDataService, PostgresService>();
                    
                    System.Diagnostics.Debug.WriteLine("🔵 App: Configuring Cloudinary service...");
                    
                    // Add Cloudinary Service
                    var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL") 
                        ?? "cloudinary://your_api_key:your_api_secret@your_cloud_name";
                    
                    System.Diagnostics.Debug.WriteLine($"🔵 App: Cloudinary URL: {(string.IsNullOrEmpty(cloudinaryUrl) ? "NOT SET" : "SET")}");
                    
                    services.AddSingleton<my_shop.Application.Interfaces.ICloudinaryService>(
                        provider => new my_shop.Infrastructure.Services.CloudinaryService(cloudinaryUrl));
                    
                    System.Diagnostics.Debug.WriteLine("🔵 App: Services configured successfully");
                }).Build();
                
                System.Diagnostics.Debug.WriteLine("🔵 App: Constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ App Constructor Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception
            System.Diagnostics.Debug.WriteLine($"❌ UNHANDLED EXCEPTION: {e.Exception.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"❌ MESSAGE: {e.Exception.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ STACK TRACE: {e.Exception.StackTrace}");
            
            if (e.Exception.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ INNER EXCEPTION: {e.Exception.InnerException.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ INNER STACK TRACE: {e.Exception.InnerException.StackTrace}");
            }
            
            // Mark as handled to prevent crash (for debugging)
            e.Handled = true;
            
            // Show error dialog
            var dialog = new ContentDialog
            {
                Title = "Lỗi ứng dụng",
                Content = $"Đã xảy ra lỗi:\n\n{e.Exception.GetType().Name}\n{e.Exception.Message}",
                CloseButtonText = "Đóng",
                XamlRoot = _window?.Content?.XamlRoot
            };
            
            if (dialog.XamlRoot != null)
            {
                _ = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = AppHost.Services.GetRequiredService<MainWindow>();
            _window.Activate();
        }

        public static T GetService<T>() where T : class
        {
            if ((App.Current as App)!.AppHost.Services.GetRequiredService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
            }
            return service;
        }
    }
}
