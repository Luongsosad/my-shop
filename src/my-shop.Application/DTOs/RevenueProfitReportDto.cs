namespace my_shop.Application.DTOs;

public class RevenueProfitDataPoint
{
    public string Period { get; set; } = string.Empty; // "2024-01-15" or "Week 3" or "Jan 2024"
    public DateTime Date { get; set; }
    public long Revenue { get; set; }
    public long Profit { get; set; }
    public long Cost { get; set; }
    public int OrderCount { get; set; }
}

public class RevenueProfitReportDto
{
    public long TotalRevenue { get; set; }
    public long TotalProfit { get; set; }
    public long TotalCost { get; set; }
    public int TotalOrders { get; set; }
    public double AverageOrderValue { get; set; }
    public double ProfitMargin { get; set; } // Percentage
    public List<RevenueProfitDataPoint> DataPoints { get; set; } = new();
}
