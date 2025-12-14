using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class DashboardView : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<DashboardViewModel>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
