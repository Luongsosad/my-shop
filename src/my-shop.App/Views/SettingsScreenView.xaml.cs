using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;
using System;
using System.Diagnostics;

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
                ViewModel.OnLogoutRequested = NavigateToLogin;
                this.DataContext = ViewModel;
                this.Loaded += async (s, e) =>
                {
                    try
                    {
                        ViewModel.XamlRoot = this.XamlRoot;
                        await ViewModel.LoadLicenseStatusAsync();
                    }
                    catch (Exception ex)
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                }
                throw;
            }
        }

        private void NavigateToLogin()
        {
            var app = Microsoft.UI.Xaml.Application.Current as App;
            var mainWindow = app?.AppHost.Services.GetRequiredService<MainWindow>();

            mainWindow?.PerformLogout();
        }

    }
}