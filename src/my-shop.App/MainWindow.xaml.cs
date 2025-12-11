using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using my_shop.App.Views;

namespace my_shop.App
{
    /// <summary>
    /// Main application window with navigation.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MainLayout _mainLayout;
        
        public MainWindow(IHost appHost)
        {
            InitializeComponent();
            
            // Load MainLayout instead of LoginScreen for now
            // TODO: Implement authentication flow later
            _mainLayout = appHost.Services.GetRequiredService<MainLayout>();
            RootGrid.Children.Add(_mainLayout);
            
            // Set window title and size
            Title = "My Shop - Management System";
            
            // Get window handle and set minimum size
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            
            if (appWindow != null)
            {
                appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1400, Height = 900 });
            }
        }
    }
}
