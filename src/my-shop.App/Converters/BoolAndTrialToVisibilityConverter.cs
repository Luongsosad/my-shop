using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    // Converter này cần 2 giá trị: IsPaid và license status
    // Nhưng WinUI không hỗ trợ MultiValueConverter tốt
    // Nên ta sẽ dùng cách khác: pass Order object và check trong converter

    public class OrderPrintBadgeVisibilityConverter : IValueConverter
    {
        private static ViewModels.OrdersScreenViewModel? _cachedViewModel;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // value = Order object
            // parameter = "Trial" or "Expired"

            if (value is not Models.Order order)
                return Visibility.Collapsed;

            // Chỉ hiện badge khi order đã paid
            if (!order.IsPaid)
                return Visibility.Collapsed;

            try
            {
                // Cache ViewModel để tránh gọi GetService nhiều lần
                _cachedViewModel ??= App.GetService<ViewModels.OrdersScreenViewModel>();

                if (parameter?.ToString() == "Trial")
                {
                    return _cachedViewModel.IsInTrial ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (parameter?.ToString() == "Expired")
                {
                    return _cachedViewModel.IsTrialExpired ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch
            {
                return Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}