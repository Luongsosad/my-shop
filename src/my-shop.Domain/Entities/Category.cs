using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities;

[Table("categories")]
public class Category
{
    [Key]
    [Column("category_id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("extension_data", TypeName = "jsonb")] 
    public string? ExtensionData { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
