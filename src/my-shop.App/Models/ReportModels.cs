using System;
using System.Collections.Generic;

namespace my_shop.App.Models;

public enum ReportPeriodType
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

public class ProductSalesDataPoint
{
    public string Period { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int QuantitySold { get; set; }
}

public class ProductSalesReport
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public long TotalRevenue { get; set; }
    public List<ProductSalesDataPoint> DataPoints { get; set; } = new();
}

public class RevenueProfitDataPoint
{
    public string Period { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public long Revenue { get; set; }
    public long Profit { get; set; }
    public long Cost { get; set; }
    public int OrderCount { get; set; }
}

public class RevenueProfitReport
{
    public long TotalRevenue { get; set; }
    public long TotalProfit { get; set; }
    public long TotalCost { get; set; }
    public int TotalOrders { get; set; }
    public double AverageOrderValue { get; set; }
    public double ProfitMargin { get; set; }
    public List<RevenueProfitDataPoint> DataPoints { get; set; } = new();
}
