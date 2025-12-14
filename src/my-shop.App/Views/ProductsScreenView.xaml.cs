using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class ProductsScreenView : Page
    {
        public ProductsScreenViewModel ViewModel { get; }

        public ProductsScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ProductsScreenViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
