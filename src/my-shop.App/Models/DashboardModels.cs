using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.UI;  // For Color
using Microsoft.UI.Xaml.Media;
using Windows.UI;  // For SolidColorBrush

namespace my_shop.App.Models
{
    public class DashboardStats
    {
        [JsonProperty("totalProducts")]
        public int TotalProducts { get; set; }
        
        [JsonProperty("todayOrders")]
        public int TodayOrders { get; set; }
        
        [JsonProperty("todayRevenue")]
        public long TodayRevenue { get; set; }
        
        [JsonProperty("lowStockCount")]
        public int LowStockCount { get; set; }
    }

    public class RecentOrderDto
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; } = string.Empty;
        
        [JsonProperty("createdTime")]
        public string CreatedTime { get; set; } = string.Empty;
        
        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }
        
        [JsonProperty("totalPrice")]
        public int TotalPrice { get; set; }

        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "created";

        public string StatusDisplayText => Status?.ToLower() switch
        {
            "created" => "Created",
            "paid" => "Paid",
            "cancel" => "Cancelled",
            _ => "Unknown"
        };

        public SolidColorBrush StatusBackgroundBrush => new SolidColorBrush(
            Status?.ToLower() switch
            {
                "created" => Color.FromArgb(255, 254, 243, 199),  // Amber-100
                "paid" => Color.FromArgb(255, 209, 250, 229),     // Green-100
                "cancel" => Color.FromArgb(255, 254, 226, 226),   // Red-100
                _ => Color.FromArgb(255, 243, 244, 246)           // Gray-100
            });

        public SolidColorBrush StatusForegroundBrush => new SolidColorBrush(
            Status?.ToLower() switch
            {
                "created" => Color.FromArgb(255, 180, 83, 9),     // Amber-700
                "paid" => Color.FromArgb(255, 21, 128, 61),       // Green-700
                "cancel" => Color.FromArgb(255, 185, 28, 28),     // Red-700
                _ => Color.FromArgb(255, 75, 85, 99)              // Gray-600
            });
    }

    public class LowStockProductDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("sku")]
        public string Sku { get; set; } = string.Empty;
        
        [JsonProperty("count")]
        public int Count { get; set; }
        
        [JsonProperty("images")]
        public List<string>? Images { get; set; }
        
        // Property ?? l?y ?nh ??u tiên ho?c placeholder
        public string FirstImageUrl => 
            Images != null && Images.Count > 0 && !string.IsNullOrEmpty(Images[0]) 
                ? Images[0] 
                : "ms-appx:///Assets/placeholder-product.png";
    }

    public class BestSellingProductDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("category")]
        public string Category { get; set; } = string.Empty;
        
        [JsonProperty("soldCount")]
        public int SoldCount { get; set; }
        
        [JsonProperty("soldPercentage")]
        public double SoldPercentage { get; set; }
        
        [JsonProperty("images")]
        public List<string>? Images { get; set; }
        
        // Property ?? l?y ?nh ??u tiên ho?c placeholder
        public string FirstImageUrl => 
            Images != null && Images.Count > 0 && !string.IsNullOrEmpty(Images[0]) 
                ? Images[0] 
                : "ms-appx:///Assets/placeholder-product.png";
    }

    public class DailyRevenueDto
    {
        [JsonProperty("day")]
        public int Day { get; set; }
        
        [JsonProperty("revenue")]
        public long Revenue { get; set; }
        
        [JsonProperty("orderCount")]
        public int OrderCount { get; set; }
    }
}
