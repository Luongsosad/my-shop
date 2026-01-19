using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using my_shop.App.ViewModels;
using System.ComponentModel;
using System.Linq;
using WinUIApp = Microsoft.UI.Xaml.Application;

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
            
            // Subscribe to SelectedQuickRange changes
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Subscribe to date changes to clear selection
            this.Loaded += ReportScreenView_Loaded;
        }

        private void ReportScreenView_Loaded(object sender, RoutedEventArgs e)
        {
            // Update button styles based on initial state
            UpdateQuickRangeButtonStyles();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SelectedQuickRange))
            {
                UpdateQuickRangeButtonStyles();
            }
        }

        private void UpdateQuickRangeButtonStyles()
        {
            var selectedRange = ViewModel.SelectedQuickRange?.ToLower();
            
            // Get all buttons in the panel
            var buttons = new[]
            {
                TodayButton,
                YesterdayButton,
                Last7DaysButton,
                Last30DaysButton,
                ThisMonthButton,
                LastMonthButton,
                ThisYearButton
            };

            foreach (var button in buttons)
            {
                var buttonTag = button.Tag?.ToString()?.ToLower();
                
                if (buttonTag == selectedRange)
                {
                    // Apply AccentButtonStyle
                    button.Style = (Style)WinUIApp.Current.Resources["AccentButtonStyle"];
                }
                else
                {
                    // Apply DefaultButtonStyle or clear style
                    button.ClearValue(StyleProperty);
                }
            }
        }
    }
}
