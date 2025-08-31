using System.ComponentModel.DataAnnotations;

namespace hua.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = [];

        [Required]
        public byte[] PasswordSalt { get; set; } = [];
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string? Role { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;
    }
}