namespace my_shop.Application.DTOs;

public class RecentOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public string CreatedTime { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalPrice { get; set; }
}
