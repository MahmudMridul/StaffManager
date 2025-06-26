using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        public int PermissionId { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public string? GrantedBy { get; set; }

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
