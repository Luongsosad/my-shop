using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class StringComparisonToStyleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return null;

            var selectedValue = value.ToString()?.ToLower();
            var compareValue = parameter.ToString()?.ToLower();

            // Return the style name as string, let XAML resolve it
            if (selectedValue == compareValue)
            {
                return true; // Selected - use AccentButtonStyle
            }

            return false; // Not selected - use default style
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
