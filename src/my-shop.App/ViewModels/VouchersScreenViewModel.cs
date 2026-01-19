using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using my_shop.App.Models;
using my_shop.App.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels;

public partial class VouchersScreenViewModel : ObservableObject
{
    private readonly VoucherService _voucherService;
    public Microsoft.UI.Xaml.XamlRoot? XamlRoot { get; set; }

    [ObservableProperty]
    private ObservableCollection<VoucherModel> _vouchers = new();

    [ObservableProperty]
    private VoucherModel? _selectedVoucher;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    // Form fields
    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int _discountPercent = 10;

    [ObservableProperty]
    private int _maxDiscountAmount = 50000;

    [ObservableProperty]
    private int _minOrderAmount = 0;

    [ObservableProperty]
    private int _totalQuantity = 100;

    [ObservableProperty]
    private int _remainingQuantity = 100;

    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now.AddDays(30);

    public VouchersScreenViewModel()
    {
        _voucherService = new VoucherService();
        _ = LoadVouchersAsync();
    }

    [RelayCommand]
    private async Task LoadVouchersAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var vouchers = await _voucherService.GetVouchersAsync();
            
            Vouchers.Clear();
            foreach (var voucher in vouchers.OrderByDescending(v => v.CreatedAt))
            {
                Vouchers.Add(voucher);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error loading voucher list: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NewVoucher()
    {
        SelectedVoucher = null;
        IsEditMode = false;
        ClearForm();
    }

    [RelayCommand]
    private void EditVoucher(VoucherModel? voucher)
    {
        System.Diagnostics.Debug.WriteLine($"🔵 EditVoucher called with voucher: {voucher?.Code ?? "null"}");
        if (voucher == null) return;

        SelectedVoucher = voucher;
        IsEditMode = true;

        Code = voucher.Code;
        Description = voucher.Description ?? string.Empty;
        DiscountPercent = voucher.DiscountPercent;
        MaxDiscountAmount = voucher.MaxDiscountAmount;
        MinOrderAmount = voucher.MinOrderAmount;
        TotalQuantity = voucher.TotalQuantity;
        RemainingQuantity = voucher.RemainingQuantity;
        StartDate = voucher.StartDate;
        EndDate = voucher.EndDate;
    }

    [RelayCommand]
    private async Task SaveVoucherAsync()
    {
        try
        {
            HasError = false;
            ErrorMessage = string.Empty;

            // Validation
            if (string.IsNullOrWhiteSpace(Code))
            {
                HasError = true;
                ErrorMessage = "Please enter the voucher code";
                return;
            }

            if (DiscountPercent <= 0 || DiscountPercent > 100)
            {
                HasError = true;
                ErrorMessage = "The percentage reduction must be between 1 and 100";
                return;
            }

            if (MaxDiscountAmount <= 0)
            {
                HasError = true;
                ErrorMessage = "The maximum discount amount must be greater than 0";
                return;
            }

            if (TotalQuantity <= 0)
            {
                HasError = true;
                ErrorMessage = "The number must be greater than 0";
                return;
            }

            if (EndDate <= StartDate)
            {
                HasError = true;
                ErrorMessage = "The end date must be after the start date";
                return;
            }

            IsLoading = true;

            var model = new VoucherModel
            {
                Code = Code.Trim().ToUpper(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                DiscountPercent = DiscountPercent,
                MaxDiscountAmount = MaxDiscountAmount,
                MinOrderAmount = MinOrderAmount,
                TotalQuantity = TotalQuantity,
                RemainingQuantity = IsEditMode ? RemainingQuantity : TotalQuantity,
                StartDate = StartDate,
                EndDate = EndDate
            };

            System.Diagnostics.Debug.WriteLine($"🔵 Creating voucher: {model.Code}, StartDate: {model.StartDate:o}, EndDate: {model.EndDate:o}");

            if (IsEditMode && SelectedVoucher != null)
            {
                model.Id = SelectedVoucher.Id;
                await _voucherService.UpdateVoucherAsync(model);
            }
            else
            {
                await _voucherService.CreateVoucherAsync(model);
            }

            await LoadVouchersAsync();
            ClearForm();
            SelectedVoucher = null;
            IsEditMode = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ SaveVoucher error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteVoucherAsync(VoucherModel? voucher)
    {
        System.Diagnostics.Debug.WriteLine($"🔵 DeleteVoucher called with voucher: {voucher?.Code ?? "null"}");
        if (voucher == null) return;

        // Show confirmation dialog
        var deleteConfirmed = await ShowDeleteConfirmationAsync(voucher);
        System.Diagnostics.Debug.WriteLine($"🔵 Delete confirmed: {deleteConfirmed}");
        if (!deleteConfirmed) return;

        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var success = await _voucherService.DeleteVoucherAsync(voucher.Id);
            
            if (success)
            {
                await LoadVouchersAsync();
                
                if (SelectedVoucher?.Id == voucher.Id)
                {
                    ClearForm();
                    SelectedVoucher = null;
                    IsEditMode = false;
                }
            }
            else
            {
                HasError = true;
                ErrorMessage = "Vouchers cannot be deleted";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error deleting voucher: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<bool> ShowDeleteConfirmationAsync(VoucherModel voucher)
    {
        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Confirm deletion",
            Content = $"Are you sure you want to delete the voucher '{voucher.Code}'?\n\nThis action cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Close,
            RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light,
            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();
        return result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        ClearForm();
        SelectedVoucher = null;
        IsEditMode = false;
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private void ClearForm()
    {
        Code = string.Empty;
        Description = string.Empty;
        DiscountPercent = 10;
        MaxDiscountAmount = 50000;
        MinOrderAmount = 0;
        TotalQuantity = 100;
        RemainingQuantity = 100;
        StartDate = DateTimeOffset.Now;
        EndDate = DateTimeOffset.Now.AddDays(30);
    }
}
