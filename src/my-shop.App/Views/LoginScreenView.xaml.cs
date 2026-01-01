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

        private void PageNavigate(Type pageType)
        {
            if (_navigationFrame == null || _appHost == null)
            {
                return;
            }

            try
            {
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
    }
}