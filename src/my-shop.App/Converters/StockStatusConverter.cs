using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class StockStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "N/A";
                
            if (value is int count)
            {
                if (count <= 0)
                    return "H?t hàng";
                else if (count <= 5)
                    return "S?p h?t";
                else
                    return "Còn hàng";
            }
            
            // Try parse if it's a string or other type
            if (int.TryParse(value.ToString(), out int parsed))
            {
                if (parsed <= 0)
                    return "H?t hàng";
                else if (parsed <= 5)
                    return "S?p h?t";
                else
                    return "Còn hàng";
            }
            
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
