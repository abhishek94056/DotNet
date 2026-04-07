using InvoiceGenerator.Models;
using InvoiceGenerator.ViewModels;

namespace InvoiceGenerator.Interfaces
{
    public interface IAuthService
    {
        // Register
        (bool success, string message) Register(RegisterViewModel vm);

        // Login
        (bool success, UserModel? user, string message) Login(LoginViewModel vm);

        // Get all users
        List<UserModel> GetAllUsers();

        // Toggle active status
        void ToggleActive(int userId, bool isActive);
    }
}
