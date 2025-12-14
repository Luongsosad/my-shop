using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class SettingsScreenView : Page
    {
        public SettingsScreenViewModel ViewModel { get; }

        public SettingsScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<SettingsScreenViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
