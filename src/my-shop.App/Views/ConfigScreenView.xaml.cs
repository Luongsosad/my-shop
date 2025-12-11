using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class ConfigScreenView : Page
    {
        public ConfigScreenViewModel ViewModel { get; }

        public ConfigScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ConfigScreenViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
