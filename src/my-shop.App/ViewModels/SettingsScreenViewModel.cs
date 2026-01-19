#nullable enable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Helpers;
using my_shop.App.Models;
using my_shop.App.Services;
using my_shop.App.Services.EmailService;
using my_shop.App.Services.LicenseService;
using my_shop.App.Services.LoginService;
using my_shop.App.Services.OnboardingService;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class SettingsScreenViewModel : ObservableObject
    {
        // ===== LICENSE MANAGEMENT DEPENDENCIES (HEAD) =====
        private readonly LicenseService _licenseService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ICredentialsService _credentialsService;
        private readonly ITokenExpirationMonitor _tokenMonitor;
        private readonly IOnboardingService _onboardingService;

        // ===== SETTINGS MANAGEMENT DEPENDENCIES (MERGE) =====
        private readonly IDataService _dataService;
        private readonly IJwtHelper _jwtHelper;

        // ===== LICENSE PROPERTIES (HEAD) =====
        [ObservableProperty]
        private bool isActivated;

        [ObservableProperty]
        private bool isInTrial;

        [ObservableProperty]
        private int daysRemaining;

        [ObservableProperty]
        private string licenseStatus = string.Empty;

        [ObservableProperty]
        private string activationCode = string.Empty;

        [ObservableProperty]
        private string generatedCodeForAdmin = string.Empty;

        public Action? OnLogoutRequested { get; set; }
        public Action? OnShowTourRequested { get; set; }
        public XamlRoot? XamlRoot { get; set; }

        // ===== SETTINGS PROPERTIES (MERGE) =====
        [ObservableProperty]
        private StoreInfoModel _storeInfo = new();

        [ObservableProperty]
        private UserProfileModel _userProfile = new();

        [ObservableProperty]
        private string _selectedTheme = "System";

        [ObservableProperty]
        private int _lowStockThreshold = 5;

        [ObservableProperty]
        private bool _notificationsEnabled = true;

        [ObservableProperty]
        private bool _soundEffectsEnabled = true;

        [ObservableProperty]
        private string _appVersion = "1.0.0";

        // ===== SHARED PROPERTIES =====
        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? _errorMessage;

        [ObservableProperty]
        private string? _successMessage;

        // Constructor with merged dependencies
        public SettingsScreenViewModel(
            LicenseService licenseService,
            IAuthService authService,
            IEmailService emailService,
            ITokenService tokenService,
            ICredentialsService credentialsService,
            ITokenExpirationMonitor tokenMonitor,
            IDataService dataService,
            IJwtHelper jwtHelper,
            IOnboardingService onboardingService)
        {
            // HEAD dependencies
            _licenseService = licenseService;
            _authService = authService;
            _emailService = emailService;
            _tokenService = tokenService;
            _credentialsService = credentialsService;
            _tokenMonitor = tokenMonitor;
            _onboardingService = onboardingService;

            // MERGE dependencies
            _dataService = dataService;
            _jwtHelper = jwtHelper;
        }

        // ===== LICENSE METHODS (HEAD) =====
        public async Task LoadLicenseStatusAsync()
        {
            try
            {
                IsLoading = true;

                if (!_authService.IsLoggedIn)
                {
                    LicenseStatus = "Please login to view license status";
                    IsActivated = false;
                    IsInTrial = false;
                    DaysRemaining = 0;
                    return;
                }

                var status = await _licenseService.CheckLicenseAsync();

                IsActivated = status.IsActivated;
                IsInTrial = status.IsInTrial;
                DaysRemaining = status.DaysRemaining;
                LicenseStatus = status.Message;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading license status: {ex.Message}");
                await ShowErrorAsync($"Error loading license status: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ActivateSoftwareAsync()
        {
            if (string.IsNullOrWhiteSpace(ActivationCode))
            {
                await ShowErrorAsync("Please enter the activation code");
                return;
            }

            if (!_authService.IsLoggedIn)
            {
                await ShowErrorAsync("Please log in first");
                return;
            }

            try
            {
                IsLoading = true;

                var success = await _licenseService.ActivateSoftwareAsync(ActivationCode);
                if (success)
                {
                    await ShowSuccessAsync("Software activation successful!");
                    await LoadLicenseStatusAsync();
                    ActivationCode = string.Empty;
                }
                else
                {
                    await ShowErrorAsync("Invalid activation code. Please check and try again..");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Activation error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void GenerateCodeForCurrentAdmin()
        {
            try
            {
                if (!_authService.IsLoggedIn)
                {
                    GeneratedCodeForAdmin = "Error: Please log in first";
                    return;
                }

                var currentAdminEmail = _authService.CurrentAdminEmail;
                GeneratedCodeForAdmin = _licenseService.GenerateActivationCode(currentAdminEmail);
            }
            catch (InvalidOperationException)
            {
                GeneratedCodeForAdmin = "Error: No admin logged in";
            }
            catch (Exception ex)
            {
                GeneratedCodeForAdmin = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task GenerateAndSendCodeAsync()
        {
            try
            {
                IsLoading = true;

                if (!_authService.IsLoggedIn)
                {
                    await ShowErrorAsync("Please log in first!");
                    return;
                }

                var currentAdminEmail = _authService.CurrentAdminEmail;
                var currentAdminName = _authService.CurrentAdminName;

                var activationCode = _licenseService.GenerateActivationCode(currentAdminEmail);
                GeneratedCodeForAdmin = activationCode;

                var emailOpened = await _emailService.SendActivationCodeAsync(
                    currentAdminEmail,
                    currentAdminName,
                    activationCode
                );

                if (emailOpened)
                {
                    await ShowSuccessAsync($"The activation code has been sent to {currentAdminEmail}. Please check your inbox!");
                }
                else
                {
                    await ShowErrorAsync("Email delivery failed. Please check your internet connection or email configuration..");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CopyActivationCode()
        {
            if (!string.IsNullOrWhiteSpace(GeneratedCodeForAdmin))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(GeneratedCodeForAdmin);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
        }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            try
            {
                // Show confirmation dialog
                if (XamlRoot == null) return;

                var dialog = new ContentDialog
                {
                    Title = "Confirm Logout",
                    Content = "Are you sure you want to log out?",
                    PrimaryButtonText = "Log Out",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = (ElementTheme)Microsoft.UI.Xaml.Application.Current.RequestedTheme
                };

                var result = await dialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }

                IsLoading = true;

                // Stop token monitoring
                _tokenMonitor.StopMonitoring();

                // Clear token
                await _tokenService.ClearTokenAsync();

                // Clear credentials
                await _credentialsService.ClearCredentialsAsync();

                // Clear auth_token from LocalSettings
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("auth_token");

                // Logout from AuthService 
                await _authService.LogoutAsync();

                Debug.WriteLine("🔵 [Logout] All data cleared successfully");

                // Navigate to login screen
                OnLogoutRequested?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Logout] Error: {ex.Message}");
                await ShowErrorAsync($"Failed to logout: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ShowOnboardingTourAsync()
        {
            try
            {
                Debug.WriteLine("🔵 [Settings] Manually showing onboarding tour");

                OnShowTourRequested?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Settings] Error showing tour: {ex.Message}");
                await ShowErrorAsync($"Failed to show tour: {ex.Message}");
            }
        }

        // ===== SETTINGS METHODS (MERGE) =====
        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                StoreInfo = await _dataService.GetStoreInfoAsync();

                var adminId = GetCurrentAdminId();
                
                if (adminId > 0)
                {
                    UserProfile = await _dataService.GetUserProfileAsync(adminId);
                    LoadUserTheme();
                }
                else
                {
                    ErrorMessage = "User information not found. Please log in again.";
                }

                var appSettings = await _dataService.GetAppSettingsAsync();
                LowStockThreshold = appSettings.LowStockThreshold;
                NotificationsEnabled = appSettings.NotificationsEnabled;
                SoundEffectsEnabled = appSettings.SoundEffectsEnabled;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Data can not loaded: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SaveStoreInfoAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                SuccessMessage = null;

                await _dataService.UpdateStoreInfoAsync(StoreInfo);
                SuccessMessage = "Store information has been saved";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    SuccessMessage = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Saving information failed: {ex.Message}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    ErrorMessage = null;
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SaveUserProfileAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                SuccessMessage = null;

                var adminId = GetCurrentAdminId();
                if (adminId > 0)
                {
                    UserProfile = await _dataService.UpdateUserProfileAsync(adminId, UserProfile.Name, null);
                    SuccessMessage = "Updated user profile!";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        SuccessMessage = null;
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Update failure user profile: {ex.Message}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    ErrorMessage = null;
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ChangePasswordAsync((string currentPassword, string newPassword, string confirmPassword) passwords)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                SuccessMessage = null;

                if (string.IsNullOrWhiteSpace(passwords.currentPassword))
                {
                    ErrorMessage = "Please enter the current password";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwords.newPassword))
                {
                    ErrorMessage = "Please enter the new password";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (passwords.newPassword != passwords.confirmPassword)
                {
                    ErrorMessage = "The verification password does not match.";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (passwords.newPassword.Length < 6)
                {
                    ErrorMessage = "The new password must have at least 6 characters!";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                var adminId = GetCurrentAdminId();
                if (adminId > 0)
                {
                    await _dataService.ChangePasswordAsync(adminId, passwords.currentPassword, passwords.newPassword);
                    SuccessMessage = "Changed password succesfully";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        SuccessMessage = null;
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to change password: {ex.Message}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    ErrorMessage = null;
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public void ChangeTheme(string theme)
        {
            try
            {
                if (SelectedTheme == theme)
                {
                    return;
                }

                SelectedTheme = theme;
                SaveUserTheme(theme);
                ApplyTheme(theme);

                SuccessMessage = $"Changed to theme {theme}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    SuccessMessage = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to change theme: {ex.Message}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    ErrorMessage = null;
                });
            }
        }

        // ===== HELPER METHODS =====
        private void LoadUserTheme()
        {
            var adminId = GetCurrentAdminId();
            if (adminId == 0)
            {
                var newTheme = "Light";
                if (SelectedTheme != newTheme)
                {
                    SelectedTheme = newTheme;
                    ApplyTheme(newTheme);
                }
                return;
            }

            string themeToApply = "Light";
            if (UserProfile != null && !string.IsNullOrEmpty(UserProfile.Theme))
            {
                themeToApply = UserProfile.Theme;
            }
            
            if (SelectedTheme != themeToApply)
            {
                SelectedTheme = themeToApply;
                ApplyTheme(themeToApply);
            }
        }

        private async void SaveUserTheme(string theme)
        {
            var adminId = GetCurrentAdminId();
            if (adminId == 0)
            {
                return;
            }

            try
            {
                await _dataService.UpdateUserProfileAsync(adminId, UserProfile.Name, theme);
                UserProfile.Theme = theme;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Can not save theme: {ex.Message}";
            }
        }

        private void ApplyTheme(string theme)
        {
            try
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
                }
            }
            catch (Exception)
            {
            }
        }

        private int GetCurrentAdminId()
        {
            try
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                
                if (localSettings.Values.TryGetValue("auth_token", out var token) && token is string jwtToken)
                {
                    var payload = _jwtHelper.DecodeToken(jwtToken);
                    
                    if (payload != null)
                    {
                        if (!string.IsNullOrEmpty(payload.AdminId) && int.TryParse(payload.AdminId, out var adminIdFromClaim))
                        {
                            localSettings.Values["admin_id"] = adminIdFromClaim;
                            return adminIdFromClaim;
                        }

                        if (!string.IsNullOrEmpty(payload.Sub) && int.TryParse(payload.Sub, out var adminIdFromSub))
                        {
                            localSettings.Values["admin_id"] = adminIdFromSub;
                            return adminIdFromSub;
                        }
                    }
                }

                if (localSettings.Values.TryGetValue("admin_id", out var savedAdminId))
                {
                    return Convert.ToInt32(savedAdminId);
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task ShowSuccessAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "Close",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowErrorAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "Close",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowInfoAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Notification",
                Content = message,
                CloseButtonText = "Close",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
