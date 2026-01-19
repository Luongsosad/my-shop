namespace my_shop.Application.DTOs;

public class LowStockProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Count { get; set; }
    public List<string>? Images { get; set; }
}
