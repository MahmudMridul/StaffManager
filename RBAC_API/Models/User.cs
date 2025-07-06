using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }

        // Navigation property for user roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
