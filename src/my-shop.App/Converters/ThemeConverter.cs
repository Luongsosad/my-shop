using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class ThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string theme && parameter is string targetTheme)
            {
                return theme.Equals(targetTheme, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked && parameter is string theme)
            {
                return theme;
            }
            return "System";
        }
    }
}
