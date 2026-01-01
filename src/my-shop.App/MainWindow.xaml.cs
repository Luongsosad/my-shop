using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Services;
using my_shop.App.Views;
using System;
using System.Diagnostics;

namespace my_shop.App
{
    public sealed partial class MainWindow : Window
    {
        private readonly IHost _appHost;
        private readonly ITokenService _tokenService;
        private readonly ITokenExpirationMonitor _tokenMonitor;
        private readonly ICredentialsService _credentialsService;
        private readonly IAuthService _authService;

        public MainWindow(
            IHost appHost,
            ITokenService tokenService,
            ITokenExpirationMonitor tokenMonitor,
            ICredentialsService credentialsService,
            IAuthService authService)
        {
            InitializeComponent();
            _appHost = appHost;
            _tokenService = tokenService;
            _tokenMonitor = tokenMonitor;
            _credentialsService = credentialsService;
            _authService = authService;


            _tokenMonitor.TokenExpired += OnTokenExpired;
            _ = InitializeAsync();

            Title = "My Shop - Management System";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1400, Height = 900 });
            }
        }

        // Handle token expiration - FORCE LOGOUT
        private void OnTokenExpired()
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Session Expired",
                    Content = "Your session has expired. Please login again.",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };

                await dialog.ShowAsync();

                var loginView = _appHost.Services.GetRequiredService<LoginScreenView>();
                MainFrame.Content = loginView;

                Debug.WriteLine("Forced logout completed, showing login screen");
            });
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            var isTokenValid = await _tokenService.IsTokenValidAsync();
            var hasRememberMe = await _credentialsService.HasRememberMeAsync();

            // ✅ Case 1: Token valid + Remember Me = AUTO LOGIN
            if (isTokenValid && hasRememberMe)
            {
                _tokenMonitor.StartMonitoring();
                var mainLayout = _appHost.Services.GetRequiredService<MainLayout>();
                MainFrame.Content = mainLayout;
            }
            // ✅ Case 2 & 3: Show Login Screen
            else
            {
                if (!isTokenValid)
                {
                    await _tokenService.ClearTokenAsync();
                }

                if (!hasRememberMe)
                {
                    await _credentialsService.ClearCredentialsAsync();
                }

                var loginView = _appHost.Services.GetRequiredService<LoginScreenView>();

                loginView.SetNavigationFrame(MainFrame, _appHost);

                MainFrame.Content = loginView;
            }
        }
    }
}