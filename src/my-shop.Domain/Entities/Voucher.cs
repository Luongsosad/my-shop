using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities;

[Table("vouchers")]
public class Voucher
{
    [Key]
    [Column("voucher_id")]
    public int Id { get; set; }

    [Required]
    [Column("code")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("discount_percent")]
    public int DiscountPercent { get; set; }

    [Column("max_discount_amount")]
    public int MaxDiscountAmount { get; set; }

    [Column("min_order_amount")]
    public int MinOrderAmount { get; set; }

    [Column("total_quantity")]
    public int TotalQuantity { get; set; }

    [Column("remaining_quantity")]
    public int RemainingQuantity { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
