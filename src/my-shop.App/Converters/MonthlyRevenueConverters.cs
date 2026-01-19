using Microsoft.UI.Xaml.Data;
using my_shop.App.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace my_shop.App.Converters
{
    public class MonthlyTotalRevenueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<DailyRevenueDto> data && data.Any())
            {
                var total = data.Sum(d => d.Revenue);
                return total.ToString("N0", new CultureInfo("vi-VN")) + " ?";
            }
            return "0 ?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MonthlyTotalOrdersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<DailyRevenueDto> data && data.Any())
            {
                var total = data.Sum(d => d.OrderCount);
                return total.ToString("N0");
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MonthlyAvgRevenueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<DailyRevenueDto> data && data.Any())
            {
                var daysWithOrders = data.Count(d => d.Revenue > 0);
                if (daysWithOrders == 0) return "0 ?";
                
                var avg = data.Sum(d => d.Revenue) / daysWithOrders;
                return avg.ToString("N0", new CultureInfo("vi-VN")) + " ?";
            }
            return "0 ?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
