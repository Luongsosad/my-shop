using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using my_shop.App.ViewModels;
using my_shop.App.Models;

namespace my_shop.App.Views;

public sealed partial class VouchersScreenView : Page
{
    public VouchersScreenViewModel ViewModel { get; } = new();

    public VouchersScreenView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) => ViewModel.XamlRoot = this.XamlRoot;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadVouchersCommand.ExecuteAsync(null);
    }

    private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Microsoft.UI.Xaml.Controls.Button button && button.Tag is VoucherModel voucher)
        {
            ViewModel.EditVoucherCommand.Execute(voucher);
        }
    }

    private async void DeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Microsoft.UI.Xaml.Controls.Button button && button.Tag is VoucherModel voucher)
        {
            await ViewModel.DeleteVoucherCommand.ExecuteAsync(voucher);
        }
    }
}
