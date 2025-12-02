using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [Column("order_id")]
    public int Id { get; set; }

    [Column("created_time")]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    [Column("final_price")]
    public int FinalPrice { get; set; }

    [Column("extension_data", TypeName = "jsonb")]
    public string? ExtensionData { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
