using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models
{
    public class Role : IdentityRole
    {
        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
