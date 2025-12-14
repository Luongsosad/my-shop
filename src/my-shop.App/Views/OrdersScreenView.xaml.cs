using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class OrdersScreenView : Page
    {
        public OrdersScreenViewModel ViewModel { get; }

        public OrdersScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<OrdersScreenViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
