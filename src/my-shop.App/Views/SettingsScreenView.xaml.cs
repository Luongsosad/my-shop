using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace my_shop.App.Views
{
    public sealed partial class SettingsScreenView : Page
    {
        public SettingsScreenViewModel ViewModel { get; }

        public SettingsScreenView()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = App.GetService<SettingsScreenViewModel>();
                
                // Set up for license management (HEAD)
                ViewModel.OnLogoutRequested = NavigateToLogin;
                
                this.DataContext = ViewModel;
                
                this.Loaded += SettingsScreenView_Loaded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ SettingsScreenView Constructor Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        private async void SettingsScreenView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.XamlRoot = this.XamlRoot;
                
                // Load license status (HEAD branch)
                await ViewModel.LoadLicenseStatusAsync();
                
                // Load settings data (MERGE branch)
                await ViewModel.LoadDataAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ SettingsScreenView_Loaded Error: {ex.Message}");
            }
        }

        // MERGE branch: Theme change handler
        private void ThemeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag is string theme)
            {
                Debug.WriteLine($"[SettingsView] Theme RadioButton clicked: {theme}");
                ViewModel.ChangeThemeCommand.Execute(theme);
            }
        }

        // MERGE branch: Change password handler
        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPassword = CurrentPasswordBox.Password;
            var newPassword = NewPasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            await ViewModel.ChangePasswordCommand.ExecuteAsync((currentPassword, newPassword, confirmPassword));

            if (string.IsNullOrEmpty(ViewModel.ErrorMessage))
            {
                CurrentPasswordBox.Password = string.Empty;
                NewPasswordBox.Password = string.Empty;
                ConfirmPasswordBox.Password = string.Empty;
            }
        }

        // MERGE branch: Logout handler
        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Xác nhận đăng xuất / Confirm Logout",
                Content = "Bạn có chắc chắn muốn đăng xuất? / Are you sure you want to log out?",
                PrimaryButtonText = "Đăng xuất / Log Out",
                CloseButtonText = "Hủy / Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    Debug.WriteLine("[SettingsView] Logout confirmed");
                    
                    // Execute logout command from MERGE branch
                    ViewModel.LogoutCommand.Execute(null);
                    Debug.WriteLine("[SettingsView] Credentials cleared");

                    // Wait a bit for cleanup
                    await Task.Delay(100);

                    // HEAD branch: Use OnLogoutRequested action for navigation
                    if (ViewModel.OnLogoutRequested != null)
                    {
                        // Also execute HEAD branch logout
                        await ViewModel.LogoutAsync();
                    }
                    else
                    {
                        // MERGE branch: Navigate using MainWindow
                        this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                        {
                            try
                            {
                                if (MainWindow.Instance != null)
                                {
                                    Debug.WriteLine("[SettingsView] Navigating via MainWindow");
                                    
                                    var mainFrame = MainWindow.Instance.GetMainFrame();
                                    if (mainFrame != null)
                                    {
                                        var loginView = App.GetService<LoginScreenView>();
                                        loginView.SetNavigationFrame(mainFrame, ((App)App.Current).AppHost);
                                        mainFrame.Content = loginView;
                                        
                                        Debug.WriteLine("[SettingsView] ✅ Navigation successful");
                                    }
                                    else
                                    {
                                        Debug.WriteLine("[SettingsView] ❌ MainFrame is null");
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("[SettingsView] ❌ MainWindow.Instance is null");
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[SettingsView] ❌ Navigation error: {ex.Message}");
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SettingsView] ❌ Logout error: {ex.Message}");
                    
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi / Error",
                        Content = $"Đăng xuất thất bại / Logout failed: {ex.Message}",
                        CloseButtonText = "Đóng / Close",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        // HEAD branch: Navigation to login
        private void NavigateToLogin()
        {
            var app = Microsoft.UI.Xaml.Application.Current as App;
            var mainWindow = app?.AppHost.Services.GetRequiredService<MainWindow>();

            mainWindow?.PerformLogout();
        }
    }
}
