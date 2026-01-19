using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Services.LoginService;
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
            try
            {
                var isTokenValid = await _tokenService.IsTokenValidAsync();
                var hasRememberMe = await _credentialsService.HasRememberMeAsync();

                Debug.WriteLine($"🔵 [InitializeAsync] Token valid: {isTokenValid}, Remember me: {hasRememberMe}");

                // Case 1: Token valid + Remember Me = AUTO LOGIN
                if (isTokenValid && hasRememberMe)
                {
                    Debug.WriteLine("🔵 [InitializeAsync] Auto-login mode - initializing current admin...");

                    // ==================== INITIALIZE CURRENT ADMIN ====================
                    var adminInitialized = await _authService.InitializeCurrentAdminFromTokenAsync();

                    if (adminInitialized)
                    {
                        Debug.WriteLine($"🔵 [InitializeAsync] Admin initialized successfully. IsLoggedIn: {_authService.IsLoggedIn}");
                        _tokenMonitor.StartMonitoring();
                        var mainLayout = _appHost.Services.GetRequiredService<MainLayout>();
                        MainFrame.Content = mainLayout;
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ [InitializeAsync] Failed to initialize admin, showing login screen");
                        await _tokenService.ClearTokenAsync();
                        await _credentialsService.ClearCredentialsAsync();
                        ShowLoginScreen();
                    }
                    // ==================================================================
                }
                // Case 2 & 3: Show Login Screen
                else
                {
                    Debug.WriteLine("🔵 [InitializeAsync] Showing login screen");

                    if (!isTokenValid)
                    {
                        await _tokenService.ClearTokenAsync();
                    }
                    if (!hasRememberMe)
                    {
                        await _credentialsService.ClearCredentialsAsync();
                    }

                    ShowLoginScreen();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [InitializeAsync] Error: {ex.Message}");
                Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                ShowLoginScreen();
            }
        }

        public void ShowLoginScreen()
        {
            var loginView = _appHost.Services.GetRequiredService<LoginScreenView>();
            loginView.SetNavigationFrame(MainFrame, _appHost);
            MainFrame.Content = loginView;
        }

        public void PerformLogout()
        {
            ShowLoginScreen();
        }



    }
}