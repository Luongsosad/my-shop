using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;
                
            if (value is int intValue)
                return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
                
            if (int.TryParse(value.ToString(), out int parsed))
                return parsed > 0 ? Visibility.Visible : Visibility.Collapsed;
                
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
