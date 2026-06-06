using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MauliFarm.Data;
using MauliFarm.Models;
using MauliFarm.Repositories.Interfaces;

namespace MauliFarm.Repositories
{
    /// <summary>
    /// Concrete implementation of IRoleRepository.
    /// Uses RoleManager + direct EF Core for summary queries.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext         _context;
        private readonly ILogger<RoleRepository>      _logger;

        public RoleRepository(
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            ILogger<RoleRepository> logger)
        {
            _roleManager = roleManager;
            _context     = context;
            _logger      = logger;
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllAsync()
            => await _context.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

        public async Task<IEnumerable<ApplicationRole>> GetAllActiveAsync()
            => await _context.Roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

        public async Task<ApplicationRole?> GetByNameAsync(string roleName)
            => await _roleManager.FindByNameAsync(roleName);

        public async Task<ApplicationRole?> GetByIdAsync(string roleId)
            => await _roleManager.FindByIdAsync(roleId);

        public async Task<bool> RoleExistsAsync(string roleName)
            => await _roleManager.RoleExistsAsync(roleName);

        public async Task<IEnumerable<(string RoleName, string Description, int UserCount)>> GetRoleSummaryAsync()
        {
            // One query via EF join for dashboard card display
            var summary = await (
                from role in _context.Roles
                join userRole in _context.UserRoles on role.Id equals userRole.RoleId into ur
                from userRole in ur.DefaultIfEmpty()
                group role by new { role.Name, role.Description } into g
                select new
                {
                    g.Key.Name,
                    Description = g.Key.Description ?? string.Empty,
                    UserCount   = g.Count(x => x.Name != null)
                }
            ).ToListAsync();

            // Recalculate accurately with distinct users
            var result = new List<(string, string, int)>();
            foreach (var item in summary)
            {
                var count = await _context.UserRoles
                    .Join(_context.Roles,
                          ur => ur.RoleId,
                          r  => r.Id,
                          (ur, r) => new { ur, r })
                    .CountAsync(x => x.r.Name == item.Name);

                result.Add((item.Name ?? string.Empty, item.Description, count));
            }

            return result.OrderBy(r => r.Item1);
        }
    }
}
