using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Handle both int and nullable int
            if (value == null)
                return false;
                
            if (value is int intValue)
                return intValue > 0;
                
            // Try parse if it's a string or other type
            if (int.TryParse(value.ToString(), out int parsed))
                return parsed > 0;
                
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
            return 0;
        }
    }
}
