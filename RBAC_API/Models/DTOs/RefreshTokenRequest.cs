using System.ComponentModel.DataAnnotations;

namespace RBAC_API.Models.DTOs
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}