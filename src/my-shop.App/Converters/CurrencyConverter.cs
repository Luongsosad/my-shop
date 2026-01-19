using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace my_shop.App.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int amount)
            {
                return amount.ToString("N0", new CultureInfo("vi-VN")) + " đ";
            }
            if (value is long longAmount)
            {
                return longAmount.ToString("N0", new CultureInfo("vi-VN")) + " đ";
            }
            return value?.ToString() ?? "0 đ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
