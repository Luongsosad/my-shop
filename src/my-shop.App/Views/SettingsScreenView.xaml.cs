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
                Title = "Xác nhận đăng xuất",
                Content = "Bạn có chắc chắn muốn đăng xuất?",
                PrimaryButtonText = "Đăng xuất",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    ResetToLightMode();
                    
                    await ViewModel.LogoutAsync();
                    
                    await Task.Delay(200);

                    this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        try
                        {
                            if (MainWindow.Instance != null)
                            {
                                var mainFrame = MainWindow.Instance.GetMainFrame();
                                if (mainFrame != null)
                                {
                                    var loginView = App.GetService<LoginScreenView>();
                                    loginView.SetNavigationFrame(mainFrame, ((App)App.Current).AppHost);
                                    mainFrame.Content = loginView;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[SettingsView] ❌ Navigation error: {ex.Message}");
                        }
                    });
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Lỗi",
                        Content = $"Đăng xuất thất bại: {ex.Message}",
                        CloseButtonText = "Đóng",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private void ResetToLightMode()
        {
            try
            {
                if (MainWindow.Instance?.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = ElementTheme.Light;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsView] Failed to reset theme: {ex.Message}");
            }
        }

        private void NavigateToLogin()
        {
            this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                try
                {
                    ResetToLightMode();
                    
                    if (MainWindow.Instance != null)
                    {
                        var mainFrame = MainWindow.Instance.GetMainFrame();
                        if (mainFrame != null)
                        {
                            var loginView = App.GetService<LoginScreenView>();
                            loginView.SetNavigationFrame(mainFrame, ((App)App.Current).AppHost);
                            mainFrame.Content = loginView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[SettingsView] ❌ NavigateToLogin error: {ex.Message}");
                }
            });
        }
    }
}
