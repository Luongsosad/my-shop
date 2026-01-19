using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Services.LoginService;
using my_shop.App.Services.OnboardingService;
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
        private readonly IOnboardingService _onboardingService;

        public static MainWindow Instance { get; private set; }
        
        public Frame GetMainFrame() => MainFrame;

        public MainWindow(
            IHost appHost,
            ITokenService tokenService,
            ITokenExpirationMonitor tokenMonitor,
            ICredentialsService credentialsService,
            IAuthService authService)
        {
            InitializeComponent();
            Instance = this;
            
            _appHost = appHost;
            _tokenService = tokenService;
            _tokenMonitor = tokenMonitor;
            _credentialsService = credentialsService;
            _authService = authService;
            _onboardingService = appHost.Services.GetRequiredService<IOnboardingService>(); 


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

        private void OnTokenExpired()
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Phiên đăng nhập hết hạn",
                    Content = "Phiên đăng nhập của bạn đã hết hạn. Vui lòng đăng nhập lại.",
                    CloseButtonText = "Đóng",
                    XamlRoot = Content.XamlRoot
                };
                await dialog.ShowAsync();

                ShowLoginScreen();
            });
        }

        private async System.Threading.Tasks.Task InitializeAsync()
        {
            try
            {
                var isTokenValid = await _tokenService.IsTokenValidAsync();
                var hasRememberMe = await _credentialsService.HasRememberMeAsync();

                Debug.WriteLine($"🔵 [InitializeAsync] Token valid: {isTokenValid}, Remember me: {hasRememberMe}");

                if (isTokenValid && hasRememberMe)
                {
                    Debug.WriteLine("🔵 [InitializeAsync] Auto-login mode - initializing current admin...");

                    var adminInitialized = await _authService.InitializeCurrentAdminFromTokenAsync();

                    if (adminInitialized)
                    {
                        Debug.WriteLine($"🔵 [InitializeAsync] Admin initialized successfully. IsLoggedIn: {_authService.IsLoggedIn}");
                        _tokenMonitor.StartMonitoring();
                        var mainLayout = _appHost.Services.GetRequiredService<MainLayout>();
                        MainFrame.Content = mainLayout;

                        await ShowOnboardingIfNeededAsync(mainLayout);
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ [InitializeAsync] Failed to initialize admin, showing login screen");
                        await _tokenService.ClearTokenAsync();
                        await _credentialsService.ClearCredentialsAsync();
                        ShowLoginScreen();
                    }
                }
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

        // ✅ THÊM METHOD MỚI
        private async System.Threading.Tasks.Task ShowOnboardingIfNeededAsync(MainLayout mainLayout)
        {
            try
            {
                var shouldShow = await _onboardingService.ShouldShowOnboardingAsync();

                if (shouldShow)
                {
                    Debug.WriteLine("🔵 [Onboarding] First time user - showing interactive tour");

                    // Wait for UI to be fully rendered
                    await System.Threading.Tasks.Task.Delay(800);

                    // Start the spotlight tour
                    await mainLayout.StartOnboardingTourAsync();

                    // Mark as completed
                    await _onboardingService.MarkOnboardingCompleteAsync();
                    Debug.WriteLine("✅ [Onboarding] Tour marked as completed");
                }
                else
                {
                    Debug.WriteLine("🔵 [Onboarding] Not first time - skipping tour");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Onboarding] Error: {ex.Message}");
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

        private async System.Threading.Tasks.Task LoadUserThemeAsync()
        {
            try
            {
                var jwtHelper = _appHost.Services.GetRequiredService<Helpers.IJwtHelper>();
                var dataService = _appHost.Services.GetRequiredService<Services.IDataService>();
                
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (!localSettings.Values.TryGetValue("auth_token", out var token) || token is not string jwtToken)
                {
                    return;
                }

                var payload = jwtHelper.DecodeToken(jwtToken);
                if (payload == null) return;

                int adminId = 0;
                if (!string.IsNullOrEmpty(payload.AdminId) && int.TryParse(payload.AdminId, out var id1))
                {
                    adminId = id1;
                }
                else if (!string.IsNullOrEmpty(payload.Sub) && int.TryParse(payload.Sub, out var id2))
                {
                    adminId = id2;
                }

                if (adminId > 0)
                {
                    var userProfile = await dataService.GetUserProfileAsync(adminId);
                    var theme = string.IsNullOrEmpty(userProfile.Theme) ? "Light" : userProfile.Theme;
                    ApplyTheme(theme);
                }
            }
            catch (Exception)
            {
                ApplyTheme("Light");
            }
        }

        private void ApplyTheme(string theme)
        {
            if (Content is FrameworkElement rootElement)
            {
                var newTheme = theme switch
                {
                    "Light" => ElementTheme.Light,
                    "Dark" => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };

                rootElement.RequestedTheme = newTheme;
            }
        }
    }
}