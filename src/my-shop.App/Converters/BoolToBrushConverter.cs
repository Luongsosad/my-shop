using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace my_shop.App.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                var options = parameter?.ToString();
                if (!string.IsNullOrEmpty(options))
                {
                    var parts = options.Split('|');
                    if (parts.Length == 2)
                    {
                        var colorString = boolValue ? parts[0] : parts[1];
                        return new SolidColorBrush(ParseColor(colorString));
                    }
                }
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private Color ParseColor(string colorString)
        {
            // Remove # if present
            colorString = colorString.TrimStart('#');
            
            if (colorString.Length == 6)
            {
                // RGB format
                byte r = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                byte g = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                byte b = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
            else if (colorString.Length == 8)
            {
                // ARGB format
                byte a = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                byte r = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                byte g = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                byte b = System.Convert.ToByte(colorString.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            
            return Colors.Black;
        }
    }
}
