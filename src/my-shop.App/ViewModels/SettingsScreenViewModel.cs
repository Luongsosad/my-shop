using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Services.LicenseService;
using my_shop.App.Services.LoginService;
using my_shop.App.Services.EmailService;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels
{
    public partial class SettingsScreenViewModel : ObservableObject
    {
        private readonly LicenseService _licenseService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ICredentialsService _credentialsService;
        private readonly ITokenExpirationMonitor _tokenMonitor;

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

        [ObservableProperty]
        private bool isLoading;

        public Action? OnLogoutRequested { get; set; }


        public XamlRoot? XamlRoot { get; set; }

        public SettingsScreenViewModel(
            LicenseService licenseService,
            IAuthService authService,
            IEmailService emailService,
            ITokenService tokenService,
            ICredentialsService credentialsService,
            ITokenExpirationMonitor tokenMonitor)
        {
            _licenseService = licenseService;
            _authService = authService;
            _emailService = emailService;
            _tokenService = tokenService;
            _credentialsService = credentialsService;
            _tokenMonitor = tokenMonitor;
        }

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
                await ShowErrorAsync("Please enter activation code");
                return;
            }

            if (!_authService.IsLoggedIn)
            {
                await ShowErrorAsync("Please login first");
                return;
            }

            try
            {
                IsLoading = true;

                var success = await _licenseService.ActivateSoftwareAsync(ActivationCode);
                if (success)
                {
                    await ShowSuccessAsync("Software activated successfully!");
                    await LoadLicenseStatusAsync();
                    ActivationCode = string.Empty;
                }
                else
                {
                    await ShowErrorAsync("Invalid activation code. Please check and try again.");
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
                    GeneratedCodeForAdmin = "Error: Please login first";
                    return;
                }

                var currentAdminEmail = _authService.CurrentAdminEmail;
                GeneratedCodeForAdmin = _licenseService.GenerateActivationCode(currentAdminEmail);

                Debug.WriteLine($"Generated activation code for {currentAdminEmail}: {GeneratedCodeForAdmin}");
            }
            catch (InvalidOperationException ex)
            {
                GeneratedCodeForAdmin = "Error: No admin logged in";
                Debug.WriteLine($"Error generating code: {ex.Message}");
            }
            catch (Exception ex)
            {
                GeneratedCodeForAdmin = $"Error: {ex.Message}";
                Debug.WriteLine($"Error generating code: {ex.Message}");
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
                    await ShowErrorAsync("Please login first");
                    return;
                }

                // Get current admin info
                var currentAdminEmail = _authService.CurrentAdminEmail;
                var currentAdminName = _authService.CurrentAdminName;

                // Generate activation code
                var activationCode = _licenseService.GenerateActivationCode(currentAdminEmail);
                GeneratedCodeForAdmin = activationCode;

                Debug.WriteLine($"🔵 Generated code for {currentAdminEmail}: {activationCode}");

                // Open email client with pre-filled content
                var emailOpened = await _emailService.SendActivationCodeAsync(
                    currentAdminEmail,
                    currentAdminName,
                    activationCode
                );

                if (emailOpened)
                {
                    await ShowSuccessAsync($"✅ Activation code has been sent to {currentAdminEmail}. Please check your inbox!");
                }
                else
                {
                    await ShowErrorAsync("❌ Failed to send email. Please check your internet connection or email configuration.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error: {ex.Message}");
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
        private async Task LogoutAsync()
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

        private async Task ShowSuccessAsync(string message)
        {
            if (XamlRoot == null) return;
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
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
                CloseButtonText = "OK",
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
                Title = "Info",
                Content = message,
                CloseButtonText = "OK",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}