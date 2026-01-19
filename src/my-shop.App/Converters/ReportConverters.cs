using Microsoft.UI.Xaml.Data;
using System;
using Windows.UI;

namespace my_shop.App.Converters
{
    public class BoolToArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isIncreased)
            {
                return isIncreased ? "\uE70E" : "\uE70D"; // Up arrow : Down arrow
            }
            return "\uE70E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    // Remove # if present
                    colorString = colorString.TrimStart('#');
                    
                    // Parse hex color
                    byte a = 255;
                    byte r = System.Convert.ToByte(colorString.Substring(0, 2), 16);
                    byte g = System.Convert.ToByte(colorString.Substring(2, 2), 16);
                    byte b = System.Convert.ToByte(colorString.Substring(4, 2), 16);
                    
                    return Color.FromArgb(a, r, g, b);
                }
                catch
                {
                    return Color.FromArgb(255, 128, 128, 128); // Gray
                }
            }
            return Color.FromArgb(255, 128, 128, 128); // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToGreenRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isPositive)
            {
                return isPositive 
                    ? Color.FromArgb(255, 76, 175, 80)   // #4CAF50 Green
                    : Color.FromArgb(255, 244, 67, 54);  // #F44336 Red
            }
            return Color.FromArgb(255, 128, 128, 128); // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToMarginStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double margin)
            {
                if (margin >= 30)
                    return "T?t (?30%)";
                else if (margin >= 15)
                    return "Trung bình (15-30%)";
                else
                    return "Th?p (<15%)";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double percentage)
            {
                return percentage.ToString("F1");
            }
            return "0.0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
