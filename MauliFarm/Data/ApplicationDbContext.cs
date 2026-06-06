using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MauliFarm.Models;

namespace MauliFarm.Data
{
    /// <summary>
    /// Main database context for Mauli Farm Management System.
    /// Inherits IdentityDbContext with custom User and Role types.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ── Core Auth Tables ──────────────────────────────────────────────
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        // ── Future module DbSets will be added below this line ────────────
        // public DbSet<Labour>          Labours          { get; set; }
        // public DbSet<LabourAttendance> LabourAttendances{ get; set; }
        // public DbSet<Expense>         Expenses         { get; set; }
        // public DbSet<HarvestRecord>   HarvestRecords   { get; set; }
        // public DbSet<Inventory>       Inventories      { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Rename default ASP.NET Identity tables ────────────────────
            builder.Entity<ApplicationUser>()       .ToTable("MF_Users");
            builder.Entity<ApplicationRole>()       .ToTable("MF_Roles");
            builder.HasDefaultSchema("dbo");

            // AspNetUserRoles etc — rename for farm namespace
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
                .ToTable("MF_UserRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>()
                .ToTable("MF_UserClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                .ToTable("MF_UserLogins");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>()
                .ToTable("MF_RoleClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
                .ToTable("MF_UserTokens");

            // ── UserActivityLog configuration ─────────────────────────────
            builder.Entity<UserActivityLog>(entity =>
            {
                entity.ToTable("MF_UserActivityLogs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ActivityType)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.IpAddress)
                      .HasMaxLength(50);

                entity.Property(e => e.UserAgent)
                      .HasMaxLength(300);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.ActivityLogs)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.ActivityType);
            });

            // ── ApplicationUser additional config ─────────────────────────
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.EmployeeCode)
                      .HasMaxLength(20);

                entity.Property(e => e.Designation)
                      .HasMaxLength(50);

                entity.Property(e => e.ProfilePicturePath)
                      .HasMaxLength(500);

                entity.Property(e => e.Address)
                      .HasMaxLength(200);

                entity.Property(e => e.Notes)
                      .HasMaxLength(500);

                entity.HasIndex(e => e.EmployeeCode)
                      .IsUnique()
                      .HasFilter("[EmployeeCode] IS NOT NULL");
            });

            // ── ApplicationRole additional config ─────────────────────────
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description)
                      .HasMaxLength(300);
            });

            // ── Seed Roles ────────────────────────────────────────────────
            SeedRoles(builder);

            // ── Seed Default SuperAdmin User ──────────────────────────────
            SeedSuperAdmin(builder);
        }

        // ─────────────────────────────────────────────────────────────────
        //  SEED: Roles
        // ─────────────────────────────────────────────────────────────────
        private static void SeedRoles(ModelBuilder builder)
        {
            var roles = new List<ApplicationRole>();

            foreach (var roleName in FarmRoles.AllRoles)
            {
                roles.Add(new ApplicationRole
                {
                    Id          = roleName.ToLower().Replace(" ", "_"),
                    Name        = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = FarmRoles.RoleDescriptions.TryGetValue(roleName, out var desc) ? desc : "",
                    CreatedOn   = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive    = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }

            builder.Entity<ApplicationRole>().HasData(roles);
        }

        // ─────────────────────────────────────────────────────────────────
        //  SEED: SuperAdmin User
        // ─────────────────────────────────────────────────────────────────
        private static void SeedSuperAdmin(ModelBuilder builder)
        {
            const string adminId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();

            var superAdmin = new ApplicationUser
            {
                Id                 = adminId,
                UserName           = "admin",
                NormalizedUserName = "ADMIN",
                Email              = "admin@maulifarm.com",
                NormalizedEmail    = "ADMIN@MAULIFARM.COM",
                EmailConfirmed     = true,
                FullName           = "Farm Administrator",
                EmployeeCode       = "MF-001",
                Designation        = "System Administrator",
                IsActive           = true,
                CreatedOn          = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SecurityStamp      = Guid.NewGuid().ToString(),
                ConcurrencyStamp   = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled     = false,
                LockoutEnabled       = false,
                AccessFailedCount    = 0
            };

            superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "Admin@123");

            builder.Entity<ApplicationUser>().HasData(superAdmin);

            // Assign SuperAdmin role
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                {
                    UserId = adminId,
                    RoleId = FarmRoles.SuperAdmin.ToLower().Replace(" ", "_")
                }
            );
        }
    }
}
