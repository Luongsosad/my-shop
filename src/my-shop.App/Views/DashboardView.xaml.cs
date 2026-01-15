using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using my_shop.App.ViewModels;
using System;

namespace my_shop.App.Views
{
    public sealed partial class DashboardView : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardView()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: Constructor START");
                
                // CRITICAL: Get ViewModel BEFORE InitializeComponent
                ViewModel = App.GetService<DashboardViewModel>();
                System.Diagnostics.Debug.WriteLine($"🔵 DashboardView: ViewModel created. Stats null? {ViewModel.Stats == null}");
                
                this.InitializeComponent();
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: InitializeComponent DONE");
                
                // Set DataContext explicitly
                this.DataContext = ViewModel;
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: DataContext set");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DashboardView Constructor Error: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            try
            {
                System.Diagnostics.Debug.WriteLine("🔵 DashboardView: OnNavigatedTo START");
                System.Diagnostics.Debug.WriteLine($"🔵 Stats before load: TotalProducts={ViewModel.Stats?.TotalProducts ?? -1}");
                
                await ViewModel.LoadDataAsync();
                
                System.Diagnostics.Debug.WriteLine($"🔵 Stats after load: TotalProducts={ViewModel.Stats?.TotalProducts ?? -1}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ OnNavigatedTo Error: {ex.Message}");
            }
        }
    }
}
