using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace my_shop.App.Converters
{
    public class StringToInfoBarSeverityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string message && !string.IsNullOrEmpty(message))
            {
                // Check if message contains error keywords
                if (message.Contains("l?i", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("không th?", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("failed", StringComparison.OrdinalIgnoreCase))
                {
                    return InfoBarSeverity.Error;
                }
                
                // Check if message contains warning keywords
                if (message.Contains("c?nh báo", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("chú ý", StringComparison.OrdinalIgnoreCase))
                {
                    return InfoBarSeverity.Warning;
                }
                
                // Check if message contains success keywords
                if (message.Contains("thành công", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("success", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("hoàn t?t", StringComparison.OrdinalIgnoreCase) ||
                    message.Contains("?ã ???c t?o", StringComparison.OrdinalIgnoreCase))
                {
                    return InfoBarSeverity.Success;
                }
            }
            
            // Default to Informational
            return InfoBarSeverity.Informational;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
