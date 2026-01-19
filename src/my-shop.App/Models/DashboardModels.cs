using System;
using Newtonsoft.Json;

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
    }

    public class LowStockProductDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("sku")]
        public string Sku { get; set; } = string.Empty;
        
        [JsonProperty("count")]
        public int Count { get; set; }
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
