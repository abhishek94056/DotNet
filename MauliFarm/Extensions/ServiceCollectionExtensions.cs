using MauliFarm.Repositories;
using MauliFarm.Repositories.Interfaces;
using MauliFarm.Services;
using MauliFarm.Services.Interfaces;

namespace MauliFarm.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection.
    /// Centralises all service and repository registrations per module.
    /// Add one line per module in Program.cs:
    ///     builder.Services.AddMauliFarmServices();
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        // ── Authentication Module ─────────────────────────────────────────
        public static IServiceCollection AddAuthModuleServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository,        UserRepository>();
            services.AddScoped<IRoleRepository,        RoleRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

            // Services
            services.AddScoped<IAuthService,        AuthService>();
            services.AddScoped<IFileUploadService,  FileUploadService>();

            return services;
        }

        // ── Placeholder for future module registrations ───────────────────
        // public static IServiceCollection AddLabourModuleServices(this IServiceCollection services)
        // {
        //     services.AddScoped<ILabourRepository, LabourRepository>();
        //     services.AddScoped<ILabourService,    LabourService>();
        //     return services;
        // }
    }
}
