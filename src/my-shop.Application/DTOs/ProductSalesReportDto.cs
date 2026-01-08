namespace my_shop.Application.DTOs;

public class ProductSalesDataPoint
{
    public string Period { get; set; } = string.Empty; // "2024-01-15" or "Week 3" or "Jan 2024"
    public DateTime Date { get; set; }
    public int QuantitySold { get; set; }
}

public class ProductSalesReportDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public long TotalRevenue { get; set; }
    public List<ProductSalesDataPoint> DataPoints { get; set; } = new();
}
