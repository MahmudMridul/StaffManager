using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models.DTOs
{
    public class SigninRequest
    {
        [Required(ErrorMessage = "Email or Username is required")]
        [StringLength(320, ErrorMessage = "Email or Username cannot exceed 320 characters")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}