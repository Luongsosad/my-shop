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
            IJwtHelper jwtHelper)
        {
            // HEAD dependencies
            _licenseService = licenseService;
            _authService = authService;
            _emailService = emailService;
            _tokenService = tokenService;
            _credentialsService = credentialsService;
            _tokenMonitor = tokenMonitor;

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
                await ShowErrorAsync("Vui lòng nhập mã kích hoạt");
                return;
            }

            if (!_authService.IsLoggedIn)
            {
                await ShowErrorAsync("Vui lòng đăng nhập trước");
                return;
            }

            try
            {
                IsLoading = true;

                var success = await _licenseService.ActivateSoftwareAsync(ActivationCode);
                if (success)
                {
                    await ShowSuccessAsync("Kích hoạt phần mềm thành công!");
                    await LoadLicenseStatusAsync();
                    ActivationCode = string.Empty;
                }
                else
                {
                    await ShowErrorAsync("Mã kích hoạt không hợp lệ. Vui lòng kiểm tra và thử lại.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Lỗi kích hoạt: {ex.Message}");
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
                    GeneratedCodeForAdmin = "Lỗi: Vui lòng đăng nhập trước";
                    return;
                }

                var currentAdminEmail = _authService.CurrentAdminEmail;
                GeneratedCodeForAdmin = _licenseService.GenerateActivationCode(currentAdminEmail);
            }
            catch (InvalidOperationException)
            {
                GeneratedCodeForAdmin = "Lỗi: Không có admin đăng nhập";
            }
            catch (Exception ex)
            {
                GeneratedCodeForAdmin = $"Lỗi: {ex.Message}";
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
                    await ShowErrorAsync("Vui lòng đăng nhập trước");
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
                    await ShowSuccessAsync($"✅ Mã kích hoạt đã được gửi đến {currentAdminEmail}. Vui lòng kiểm tra hộp thư!");
                }
                else
                {
                    await ShowErrorAsync("❌ Gửi email thất bại. Vui lòng kiểm tra kết nối internet hoặc cấu hình email.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Lỗi: {ex.Message}");
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
                IsLoading = true;

                _tokenMonitor.StopMonitoring();
                await _tokenService.ClearTokenAsync();
                await _credentialsService.ClearCredentialsAsync();
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("auth_token");
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("admin_id");
                await _authService.LogoutAsync();

                OnLogoutRequested?.Invoke();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Đăng xuất thất bại: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
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
                    ErrorMessage = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.";
                }

                var appSettings = await _dataService.GetAppSettingsAsync();
                LowStockThreshold = appSettings.LowStockThreshold;
                NotificationsEnabled = appSettings.NotificationsEnabled;
                SoundEffectsEnabled = appSettings.SoundEffectsEnabled;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Không thể tải dữ liệu: {ex.Message}";
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
                SuccessMessage = "Đã lưu thông tin cửa hàng";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    SuccessMessage = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lưu thông tin thất bại: {ex.Message}";
                
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
                    SuccessMessage = "Đã cập nhật hồ sơ";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        SuccessMessage = null;
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Cập nhật hồ sơ thất bại: {ex.Message}";
                
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
                    ErrorMessage = "Vui lòng nhập mật khẩu hiện tại";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwords.newPassword))
                {
                    ErrorMessage = "Vui lòng nhập mật khẩu mới";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (passwords.newPassword != passwords.confirmPassword)
                {
                    ErrorMessage = "Mật khẩu xác nhận không khớp";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        ErrorMessage = null;
                    });
                    return;
                }

                if (passwords.newPassword.Length < 6)
                {
                    ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự";
                    
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
                    SuccessMessage = "Đã đổi mật khẩu thành công";
                    
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        SuccessMessage = null;
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đổi mật khẩu thất bại: {ex.Message}";
                
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

                SuccessMessage = $"Đã chuyển sang theme {theme}";
                
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    SuccessMessage = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Thay đổi theme thất bại: {ex.Message}";
                
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
                ErrorMessage = $"Không thể lưu theme: {ex.Message}";
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
                Title = "Thành công",
                Content = message,
                CloseButtonText = "Đóng",
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
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "Đóng",
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
                Title = "Thông báo",
                Content = message,
                CloseButtonText = "Đóng",
                RequestedTheme = ElementTheme.Light,
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
