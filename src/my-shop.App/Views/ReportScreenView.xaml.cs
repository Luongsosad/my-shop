using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;

namespace my_shop.App.Views
{
    public sealed partial class ReportScreenView : Page
    {
        public ReportScreenViewModel ViewModel { get; }

        public ReportScreenView()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ReportScreenViewModel>();
            this.DataContext = ViewModel;
        }
    }
}
