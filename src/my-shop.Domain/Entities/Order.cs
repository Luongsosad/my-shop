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

    [Column("customer_id")]
    public int? CustomerId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer? Customer { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "created";

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
