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
                return "Chưa bắt đầu";
            if (now > EndDate)
                return "Đã hết hạn";
            if (RemainingQuantity <= 0)
                return "Hết lượt";
            return "Đang hoạt động";
        }
    }

    public string DisplayUsage => $"{TotalQuantity - RemainingQuantity}/{TotalQuantity}";

    public string DisplayDiscount => $"{DiscountPercent}% (tối đa {MaxDiscountAmount:N0}đ)";

    public string DisplayMinOrder => MinOrderAmount > 0 ? $"Đơn tối thiểu {MinOrderAmount:N0}đ" : "Không giới hạn";
}
