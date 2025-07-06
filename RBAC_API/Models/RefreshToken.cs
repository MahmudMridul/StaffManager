using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public DateTime ExpiresAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? RevokedAt { get; set; }
        
        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
        
        // Navigation property
        public User User { get; set; } = null!;
    }
}