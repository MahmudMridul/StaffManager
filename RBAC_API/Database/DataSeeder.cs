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
                    Id = "550e8400-e29b-41d4-a716-446655440001",
                    Name = "Super Admin",
                    NormalizedName = "SUPER_ADMIN",
                    Description = "Full system access with all permissions",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    ConcurrencyStamp = "super-admin-stamp"
                },
                new Role
                {
                    Id = "550e8400-e29b-41d4-a716-446655440002",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrative access with most permissions",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    ConcurrencyStamp = "admin-stamp"
                },
                new Role
                {
                    Id = "550e8400-e29b-41d4-a716-446655440003",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "Management level access",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    ConcurrencyStamp = "manager-stamp"
                },
                new Role
                {
                    Id = "550e8400-e29b-41d4-a716-446655440004",
                    Name = "Senior Staff",
                    NormalizedName = "SENIOR_STAFF",
                    Description = "Senior level staff access",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    ConcurrencyStamp = "senior-staff-stamp"
                },
                new Role
                {
                    Id = "550e8400-e29b-41d4-a716-446655440005",
                    Name = "Junior Staff",
                    NormalizedName = "JUNIOR_STAFF",
                    Description = "Junior level staff access",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    ConcurrencyStamp = "junior-staff-stamp"
                }
            };

            builder.Entity<Role>().HasData(roles);
        }
    }
}
