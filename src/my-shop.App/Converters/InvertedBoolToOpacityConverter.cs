using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class InvertedBoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 0.3 : 1.0;
            }
            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
