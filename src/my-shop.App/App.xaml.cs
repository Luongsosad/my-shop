using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Helpers;
using my_shop.App.Repositories;
using my_shop.App.Services;
using my_shop.App.Services.EmailService;
using my_shop.App.Services.LicenseService;
using my_shop.App.Services.LoginService;
using my_shop.App.ViewModels;
using my_shop.App.Views;
using my_shop.Application.Interfaces;
using my_shop.Application.Services;
using my_shop.Infrastructure.Services;
using System;
using System.IO;
using System.Net.Http;

namespace my_shop.App
{
    public partial class App : Microsoft.UI.Xaml.Application
    {
        private Window? _window;
        public IHost AppHost { get; private set; }

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
                    foreach (var line in System.IO.File.ReadAllLines(envPath))
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
                        var parts = line.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
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
                    // ==================== HTTP CLIENT - SINGLETON ====================
                    services.AddSingleton<HttpClient>(sp =>
                    {
                        var client = new HttpClient
                        {
                            BaseAddress = new Uri("http://localhost:5000")
                        };
                        System.Diagnostics.Debug.WriteLine($"🔵 HttpClient created with BaseAddress: {client.BaseAddress}");
                        return client;
                    });
                    // =================================================================

                    services.AddTransient<MainWindow>();
                    services.AddTransient<MainLayout>();

                    services.AddTransient<ConfigScreenView>();
                    services.AddTransient<CustomersScreenView>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<LoginScreenView>();
                    services.AddTransient<OrdersScreenView>();
                    services.AddTransient<ProductsScreenView>();
                    services.AddTransient<ReportScreenView>();
                    services.AddTransient<SettingsScreenView>();
                    services.AddTransient<SignupView>();

                    services.AddTransient<ConfigScreenViewModel>();
                    services.AddTransient<CustomersScreenViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<LoginScreenViewModel>();
                    services.AddTransient<OrdersScreenViewModel>();
                    services.AddTransient<ProductsScreenViewModel>();
                    services.AddTransient<ReportScreenViewModel>();
                    services.AddTransient<SettingsScreenViewModel>();
                    services.AddTransient<SignupViewModel>();

                    // ==================== AUTH & TOKEN SERVICES ====================
                    services.AddSingleton<IJwtHelper, JwtHelper>();
                    services.AddSingleton<ITokenService, TokenService>();
                    services.AddSingleton<ICredentialsService, CredentialsService>();
                    services.AddSingleton<ITokenExpirationMonitor, TokenExpirationMonitor>();

                    // CRITICAL: AuthService MUST be registered as SINGLETON
                    services.AddSingleton<AuthService>();
                    services.AddSingleton<IAuthService>(sp =>
                    {
                        var authService = sp.GetRequiredService<AuthService>();
                        System.Diagnostics.Debug.WriteLine($"🔵 IAuthService resolved to AuthService instance: {authService.GetHashCode()}");
                        return authService;
                    });
                    // ===============================================================

                    // ==================== OTHER SERVICES ====================
                    services.AddSingleton<IDataService, PostgresService>();
                    services.AddTransient<my_shop.App.Services.OrdersService.OrderService>();
                    services.AddSingleton<IEmailService>(sp =>
                    {
                        var gmailEmail = "heyhey24122003zzz@gmail.com"; 
                        var gmailAppPassword = "ambw dtag mbmp kpox"; 

                        return new AutoEmailService(gmailEmail, gmailAppPassword);
                    });


                    // License System
                    services.AddSingleton<IAdminRepository, AdminRepository>();
                    services.AddSingleton<LicenseService>();
                    // ========================================================

                    System.Diagnostics.Debug.WriteLine("🔵 App: Configuring Cloudinary service...");

                    // Add Cloudinary Service
                    var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL")
                        ?? "cloudinary://your_api_key:your_api_secret@your_cloud_name";

                    System.Diagnostics.Debug.WriteLine($"🔵 App: Cloudinary URL: {(string.IsNullOrEmpty(cloudinaryUrl) ? "NOT SET" : "SET")}");

                    services.AddSingleton<my_shop.Application.Interfaces.ICloudinaryService>(
                        provider => new my_shop.Infrastructure.Services.CloudinaryService(cloudinaryUrl));

                    // Signup Service with HttpClient
                    services.AddHttpClient<ISignupService, SignupService>(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5000");
                    });

                    System.Diagnostics.Debug.WriteLine("🔵 App: Services configured successfully");
                }).Build();

                System.Diagnostics.Debug.WriteLine("🔵 App: Constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ App Constructor Error: {ex.Message}");
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

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var tokenService = AppHost.Services.GetRequiredService<ITokenService>();
            var jwtHelper = AppHost.Services.GetRequiredService<IJwtHelper>();
            var credentialsService = AppHost.Services.GetRequiredService<ICredentialsService>();
            var authService = AppHost.Services.GetRequiredService<IAuthService>();
            var tokenMonitor = new TokenExpirationMonitor(tokenService, jwtHelper);

            System.Diagnostics.Debug.WriteLine($"🔵 OnLaunched: AuthService instance: {authService.GetHashCode()}");

            _window = new MainWindow(AppHost, tokenService, tokenMonitor, credentialsService, authService);
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