using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace my_shop.App.Models;

public partial class VoucherModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private int _discountPercent;

    [ObservableProperty]
    private int _maxDiscountAmount;

    [ObservableProperty]
    private int _minOrderAmount;

    [ObservableProperty]
    private int _totalQuantity;

    [ObservableProperty]
    private int _remainingQuantity;

    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now;

    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now.AddDays(30);

    [ObservableProperty]
    private bool _isDeleted;

    [ObservableProperty]
    private DateTimeOffset _createdAt;

    public string DisplayStatus
    {
        get
        {
            var now = DateTime.Now;
            if (now < StartDate)
                return "Haven't started yet";
            if (now > EndDate)
                return "Expired";
            if (RemainingQuantity <= 0)
                return "Out of stock";
            return "Active";
        }
    }

    public string DisplayUsage => $"{TotalQuantity - RemainingQuantity}/{TotalQuantity}";

    public string DisplayDiscount => $"{DiscountPercent}% (max {MaxDiscountAmount:N0}đ)";

    public string DisplayMinOrder => MinOrderAmount > 0 ? $"Minimum order {MinOrderAmount:N0}đ" : "Unlimited";
}
