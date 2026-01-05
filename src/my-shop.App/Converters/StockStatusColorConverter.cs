using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace my_shop.App.Converters
{
    public class StockStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return new SolidColorBrush(Colors.Gray);
                
            if (value is int count)
            {
                if (count <= 0)
                    return new SolidColorBrush(Colors.Red);
                else if (count <= 5)
                    return new SolidColorBrush(Colors.Orange);
                else
                    return new SolidColorBrush(Colors.Green);
            }
            
            // Try parse if it's a string or other type
            if (int.TryParse(value?.ToString(), out int parsed))
            {
                if (parsed <= 0)
                    return new SolidColorBrush(Colors.Red);
                else if (parsed <= 5)
                    return new SolidColorBrush(Colors.Orange);
                else
                    return new SolidColorBrush(Colors.Green);
            }
            
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
