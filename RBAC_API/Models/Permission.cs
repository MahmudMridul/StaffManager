using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models
{
    public class Permission
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Resource { get; set; } = string.Empty; // e.g., "Employee", "Department"

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // e.g., "Create", "Read", "Update", "Delete"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
