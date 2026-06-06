using Microsoft.AspNetCore.Identity;
using MauliFarm.Models;

namespace MauliFarm.Data
{
    /// <summary>
    /// Handles runtime database initialization, migration, and seeding.
    /// Called once at application startup.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context     = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                var logger      = services.GetRequiredService<ILogger<ApplicationDbContext>>();

                // Apply any pending migrations automatically
                await context.Database.MigrateAsync();

                logger.LogInformation("Mauli Farm DB migration applied successfully.");

                // Ensure all roles exist (fallback in case migration seed didn't run)
                await EnsureRolesAsync(roleManager, logger);

                // Ensure SuperAdmin exists
                await EnsureSuperAdminAsync(userManager, logger);

                logger.LogInformation("Mauli Farm DB initialization complete.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while initializing the Mauli Farm database.");
                throw;
            }
        }

        private static async Task EnsureRolesAsync(
            RoleManager<ApplicationRole> roleManager,
            ILogger logger)
        {
            foreach (var roleName in FarmRoles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name        = roleName,
                        Description = FarmRoles.RoleDescriptions.TryGetValue(roleName, out var d) ? d : "",
                        IsActive    = true
                    };

                    var result = await roleManager.CreateAsync(role);

                    if (result.Succeeded)
                        logger.LogInformation("Role '{Role}' created.", roleName);
                    else
                        logger.LogWarning("Failed to create role '{Role}': {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        private static async Task EnsureSuperAdminAsync(
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            const string adminEmail    = "admin@maulifarm.com";
            const string adminUserName = "admin";
            const string adminPassword = "Admin@123";

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing != null) return;

            var superAdmin = new ApplicationUser
            {
                UserName      = adminUserName,
                Email         = adminEmail,
                FullName      = "Farm Administrator",
                EmployeeCode  = "MF-001",
                Designation   = "System Administrator",
                IsActive      = true,
                EmailConfirmed = true,
                CreatedOn     = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(superAdmin, adminPassword);

            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, FarmRoles.SuperAdmin);
                logger.LogInformation("SuperAdmin user created: {Email}", adminEmail);
            }
            else
            {
                logger.LogWarning("Failed to create SuperAdmin: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
