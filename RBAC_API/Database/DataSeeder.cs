using Microsoft.EntityFrameworkCore;
using RBAC_API.Models;

namespace RBAC_API.Database
{
    public class DataSeeder
    {
        public static void SeedData(ModelBuilder builder)
        {
            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            var roles = new[]
            {
                new Role
                {
                    Id = "1",
                    Name = "Super Admin",
                    NormalizedName = "SUPER_ADMIN",
                    Description = "Full system access with all permissions",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ConcurrencyStamp = "super-admin-stamp"
                },
                new Role
                {
                    Id = "2",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrative access with most permissions",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ConcurrencyStamp = "admin-stamp"
                },
                new Role
                {
                    Id = "3",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Management level access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ConcurrencyStamp = "manager-stamp"
                },
                new Role
                {
                    Id = "4",
                    Name = "Senior Staff",
                    NormalizedName = "SENIOR_STAFF",
                    Description = "Senior level staff access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ConcurrencyStamp = "senior-staff-stamp"
                },
                new Role
                {
                    Id = "5",
                    Name = "Junior Staff",
                    NormalizedName = "JUNIOR_STAFF",
                    Description = "Junior level staff access",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ConcurrencyStamp = "junior-staff-stamp"
                }
            };

            builder.Entity<Role>().HasData(roles);
        }
    }
}
