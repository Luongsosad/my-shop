namespace my_shop.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ImportPrice { get; set; }
    public int Count { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<string>? Images { get; set; }
}
