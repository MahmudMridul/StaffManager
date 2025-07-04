using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RBAC_API.Common;
using RBAC_API.Models;
using RBAC_API.Models.DTOs;

namespace RBAC_API.Servies
{
    public class SignupValidationService
    {
        private readonly UserManager<User> _userManager;

        public SignupValidationService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ValidationResult> ValidateSignupAsync(SignupRequest request)
        {
            var result = new ValidationResult();

            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName || u.Email == request.Email);

            if (existingUser != null)
            {
                string conflictField = existingUser.UserName == request.UserName ? "Username" : "Email";
                result.AddError($"{conflictField} already exists");
            }

            if (IsCommonPassword(request.Password))
            {
                result.AddError("Password is too common. Please choose a stronger password.");
            }

            if (HasRestrictedEmailDomain(request.Email))
            {
                result.AddError("Email domain is not allowed for registration.");
            }

            if (ContainsPersonalInfo(request.Password, request.FirstName, request.LastName, request.UserName))
            {
                result.AddError("Password cannot contain personal information.");
            }

            return result;
        }

        private bool IsCommonPassword(string password)
        {
            var commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "password", "123456", "password123", "admin", "qwerty", "welcome",
                "letmein", "monkey", "1234567890", "password1", "123456789"
            };

            return commonPasswords.Contains(password);
        }

        private bool HasRestrictedEmailDomain(string email)
        {
            var restrictedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "tempmail.com", "10minutemail.com", "guerrillamail.com"
            };

            var domain = email.Split('@').LastOrDefault();
            return domain != null && restrictedDomains.Contains(domain);
        }

        private bool ContainsPersonalInfo(string password, string firstName, string lastName, string userName)
        {
            var personalInfo = new[] { firstName, lastName, userName };
            return personalInfo.Any(info =>
                !string.IsNullOrEmpty(info) &&
                password.Contains(info, StringComparison.OrdinalIgnoreCase));
        }
    }
}
