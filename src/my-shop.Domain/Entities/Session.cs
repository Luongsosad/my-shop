using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities;

[Table("sessions")]
public class Session
{
    [Key]
    [Column("session_id")]
    public int Id { get; set; }

    [Required]
    [Column("admin_id")]
    public int AdminId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("token")]
    public string Token { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(AdminId))]
    public Admin? Admin { get; set; }
}
