using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_shop.App.Converters
{
    public class BoolToExpandIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isExpanded)
            {
                // Unicode cho mũi tên
                return isExpanded ? "\uE70D" : "\uE70E"; // ChevronDown : ChevronRight
            }
            return "\uE70E";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
