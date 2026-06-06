using MauliFarm.Models;

namespace MauliFarm.Repositories.Interfaces
{
    /// <summary>
    /// Contract for role management data access.
    /// </summary>
    public interface IRoleRepository
    {
        Task<IEnumerable<ApplicationRole>> GetAllAsync();
        Task<IEnumerable<ApplicationRole>> GetAllActiveAsync();
        Task<ApplicationRole?>             GetByNameAsync(string roleName);
        Task<ApplicationRole?>             GetByIdAsync(string roleId);
        Task<bool>                         RoleExistsAsync(string roleName);

        // Returns list of (RoleName, UserCount) for admin dashboard
        Task<IEnumerable<(string RoleName, string Description, int UserCount)>> GetRoleSummaryAsync();
    }
}
