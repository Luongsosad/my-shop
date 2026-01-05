using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace my_shop.App.Converters
{
    public class ImageListToFirstImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Log the actual type for debugging
                System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] Input type: {value?.GetType()?.FullName ?? "null"}");
                
                // Handle null - return null to show placeholder
                if (value == null)
                {
                    System.Diagnostics.Debug.WriteLine("[ImageListToFirstImageConverter] Value is null");
                    return null;
                }
                
                // Handle List<string>
                if (value is List<string> list && list.Count > 0)
                {
                    var firstImage = list.First();
                    System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] List<string> with {list.Count} items, first: {firstImage}");
                    if (!string.IsNullOrWhiteSpace(firstImage))
                    {
                        var uri = new Uri(firstImage);
                        System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] Returning Uri: {uri}");
                        return uri;
                    }
                }
                
                // Handle any IEnumerable<string>
                if (value is IEnumerable<string> enumerable)
                {
                    var first = enumerable.FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] IEnumerable<string>, first: {first}");
                    if (!string.IsNullOrEmpty(first))
                        return new Uri(first);
                }
                
                // Handle any IEnumerable (non-generic)
                if (value is IEnumerable enumerableNonGeneric)
                {
                    System.Diagnostics.Debug.WriteLine("[ImageListToFirstImageConverter] IEnumerable (non-generic)");
                    foreach (var item in enumerableNonGeneric)
                    {
                        if (item is string str && !string.IsNullOrEmpty(str))
                        {
                            System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] Found string: {str}");
                            return new Uri(str);
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine("[ImageListToFirstImageConverter] No handler matched, returning null");
                // Return null to show placeholder
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] ERROR: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ImageListToFirstImageConverter] Stack: {ex.StackTrace}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
