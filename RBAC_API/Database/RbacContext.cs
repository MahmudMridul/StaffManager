using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RBAC_API.Models;

namespace RBAC_API.Database
{
    public class RbacContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>,
        UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public RbacContext(DbContextOptions<RbacContext> options) : base(options)
        {
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("rbac");

            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "rbac");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "rbac");
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Description).HasMaxLength(500);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles", "rbac");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ur => ur.AssignedAt).HasDefaultValueSql("NOW()");
                entity.Property(ur => ur.AssignedBy).HasMaxLength(450);
            });

            builder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions", "rbac");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.Resource).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Action).IsRequired().HasMaxLength(50);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");
                entity.HasIndex(p => new { p.Resource, p.Action }).IsUnique();
                entity.HasIndex(p => p.Name);
            });

            builder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions", "rbac");
                entity.HasKey(rp => rp.Id);

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rp => rp.RoleId).IsRequired().HasMaxLength(450);
                entity.Property(rp => rp.GrantedAt).HasDefaultValueSql("NOW()");
                entity.Property(rp => rp.GrantedBy).HasMaxLength(450);

                // Ensure unique combination of Role + Permission
                entity.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
            });

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "rbac");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "rbac");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "rbac");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "rbac");
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is Role role)
                {
                    role.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
