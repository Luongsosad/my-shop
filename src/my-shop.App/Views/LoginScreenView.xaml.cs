using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;
using System;
using System.Diagnostics;

namespace my_shop.App.Views
{
    public sealed partial class LoginScreenView : Page
    {
        private bool isRevealed = false;
        private Frame? _navigationFrame;
        private IHost? _appHost;

        public LoginScreenViewModel ViewModel { get; }

        public LoginScreenView(LoginScreenViewModel loginScreenViewModel)
        {
            this.InitializeComponent();
            ViewModel = loginScreenViewModel;
            this.DataContext = ViewModel;
            ViewModel.NavigateRequested += PageNavigate;

            this.Loaded += (sender, e) =>
            {
                ViewModel.MainXamlRoot = this.XamlRoot;
            };
        }

        public void SetNavigationFrame(Frame frame, IHost appHost)
        {
            _navigationFrame = frame;
            _appHost = appHost;
            Debug.WriteLine($"Navigation frame set. Frame is null: {frame == null}");
        }

        private void RevealButton_Click(object sender, RoutedEventArgs e)
        {
            isRevealed = !isRevealed;
            PasswordBox.PasswordRevealMode = isRevealed
                ? PasswordRevealMode.Visible
                : PasswordRevealMode.Hidden;
            EyeIcon.Glyph = isRevealed ? "\xED1A" : "\uE052";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            RevealButton.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async void PageNavigate(Type pageType)
        {
            if (_navigationFrame == null || _appHost == null)
            {
                return;
            }

            try
            {
                // Load user theme before navigating
                await LoadUserThemeAsync();
                
                var page = _appHost.Services.GetService(pageType) as Page;

                if (page != null)
                {
                    _navigationFrame.Content = page;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
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
                    Debug.WriteLine($"[LoginScreen] Applied user theme after login: {theme}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoginScreen] Failed to load user theme: {ex.Message}");
                ApplyTheme("Light");
            }
        }

        private void ApplyTheme(string theme)
        {
            if (MainWindow.Instance?.Content is FrameworkElement rootElement)
            {
                var newTheme = theme switch
                {
                    "Light" => ElementTheme.Light,
                    "Dark" => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };

                rootElement.RequestedTheme = newTheme;
                Debug.WriteLine($"[LoginScreen] Theme applied: {newTheme}");
            }
        }
    }
}