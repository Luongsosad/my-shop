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
    public partial class App : Application
    {
        private Window? _window;
        public IHost AppHost { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            AppHost = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddTransient<MainWindow>();
                services.AddTransient<MainLayout>();

                // Add View Services
                services.AddTransient<ConfigScreenView>();
                services.AddTransient<DashboardView>();
                services.AddTransient<LoginScreenView>();
                services.AddTransient<OrdersScreenView>();
                services.AddTransient<ProductsScreenView>();
                services.AddTransient<ReportScreenView>();
                services.AddTransient<SettingsScreenView>();

                // Add ViewModel Services
                services.AddTransient<ConfigScreenViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<LoginScreenViewModel>();
                services.AddTransient<OrdersScreenViewModel>();
                services.AddTransient<ProductsScreenViewModel>();
                services.AddTransient<ReportScreenViewModel>();
                services.AddTransient<SettingsScreenViewModel>();

                // Add Services
                services.AddSingleton<IDataService, PostgresService>();
            }).Build();
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
