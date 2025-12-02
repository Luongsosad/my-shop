using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities;

[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("sku")]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("import_price")]
    public int ImportPrice { get; set; }

    [Column("count")]
    public int Count { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("extension_data", TypeName = "jsonb")]
    public string? ExtensionData { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}
