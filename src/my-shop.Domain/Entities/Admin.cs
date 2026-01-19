using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace my_shop.Domain.Entities
{
    [Table("admins")]
    public class Admin
    {
        [Key]
        [Column("admin_id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(100)]
        [Column("name")]
        public string? Name { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("trial_start_date")]
        public DateTimeOffset? TrialStartDate { get; set; }

        [Column("is_activated")]
        public bool IsActivated { get; set; } = false;

        [Column("activation_code")]
        [MaxLength(50)]
        public string? ActivationCode { get; set; }

        [Column("activated_at")]
        public DateTimeOffset? ActivatedAt { get; set; }
    }
}