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

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Updated { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for user roles
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
