using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIRoutedEventArgs = Microsoft.UI.Xaml.RoutedEventArgs;

namespace my_shop.App.Views
{
    public sealed partial class SignupView : UserControl
    {
        private bool isRevealed = false;

        public SignupView()
        {
            this.InitializeComponent();
        }

        private void RevealButton_Click(object sender, WinUIRoutedEventArgs e)
        {
            isRevealed = !isRevealed;
            PasswordBox.PasswordRevealMode = isRevealed
                ? PasswordRevealMode.Visible
                : PasswordRevealMode.Hidden;
            EyeIcon.Glyph = isRevealed ? "\xED1A" : "\uE052";
        }

        private void PasswordBox_PasswordChanged(object sender, WinUIRoutedEventArgs e)
        {
            RevealButton.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}