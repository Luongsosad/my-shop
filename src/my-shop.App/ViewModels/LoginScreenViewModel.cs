using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.Services.LoginService;
using my_shop.App.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace my_shop.App.ViewModels
{
    public partial class LoginScreenViewModel : ObservableObject
    {
        public XamlRoot MainXamlRoot { get; set; }
        private readonly IAuthService _authService;
        private readonly ITokenExpirationMonitor _tokenMonitor;
        private readonly ICredentialsService _credentialsService;
        public event Action<Type>? NavigateRequested;
        [ObservableProperty]
        private string _username;
        [ObservableProperty]
        private string _password;
        [ObservableProperty]
        private bool _rememberMe;
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand SignupCommand { get; }
        public LoginScreenViewModel(IAuthService authService, ITokenExpirationMonitor tokenMonitor, ICredentialsService credentialsService)
        {
            _authService = authService;
            _tokenMonitor = tokenMonitor;
            _credentialsService = credentialsService;

            LoginCommand = new AsyncRelayCommand(ExecuteLogin, CanExecuteLogin);
            ForgotPasswordCommand = new AsyncRelayCommand(ExecuteForgotPassword);
            SignupCommand = new AsyncRelayCommand(ExecuteSignup);

            _ = LoadSavedCredentialsAsync();
        }

        private async Task LoadSavedCredentialsAsync()
        {
            try
            {
                var (username, password, rememberMe) = await _credentialsService.GetCredentialsAsync();

                if (rememberMe)
                {
                    Username = username;
                    Password = password;
                    RememberMe = rememberMe;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task ExecuteLogin()
        {
            try
            {
                var result = await _authService.LoginAsync(Username, Password);

                if (RememberMe)
                {
                    await _credentialsService.SaveCredentialsAsync(Username, Password, true);
                }
                else
                {
                    await _credentialsService.ClearCredentialsAsync();
                }
                _tokenMonitor.StartMonitoring();
                NavigateRequested?.Invoke(typeof(MainLayout));

            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Login Failed",
                    Content = "Invalid username or password. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = MainXamlRoot,
                    RequestedTheme = ElementTheme.Light
                };
                await dialog.ShowAsync();
            }
        }


        private async Task ExecuteForgotPassword()
        {
            var dialog = new ContentDialog
            {
                Title = "Forgot Password",
                Content = "A password reset link will be sent to your email.",
                PrimaryButtonText = "Send",
                CloseButtonText = "Cancel",
                XamlRoot = MainXamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ExecuteSignup()
        {
            var vm = App.GetService<SignupViewModel>();
            var view = new SignupView
            {
                DataContext = vm
            };

            var dialog = new ContentDialog
            {
                Title = "Create Account",
                Content = view,
                PrimaryButtonText = "Sign Up",
                CloseButtonText = "Cancel",
                XamlRoot = MainXamlRoot,
                RequestedTheme = ElementTheme.Light
            };

            dialog.PrimaryButtonClick += async (_, __) =>
            {
                await vm.SignupCommand.ExecuteAsync(null);
            };

            await dialog.ShowAsync();
        }




        partial void OnUsernameChanged(string value)
        {
            (LoginCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }

        partial void OnPasswordChanged(string value)
        {
            (LoginCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }
}
