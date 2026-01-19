namespace my_shop.Application.DTOs;

public class BestSellingProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int SoldCount { get; set; }
    public double SoldPercentage { get; set; }
    public List<string>? Images { get; set; }
}
