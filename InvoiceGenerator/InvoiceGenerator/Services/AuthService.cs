using BCrypt.Net;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using InvoiceGenerator.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _conn;

        public AuthService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;
        // IConfiguration -> Used to read connection string from appsettings.json

        // ── Register ──
        public (bool success, string message) Register(RegisterViewModel vm)
        {
            string hashed = BCrypt.Net.BCrypt.HashPassword(vm.Password, workFactor: 11);
            // HashPassword() -> Encrypts password using salt (no need to store salt separately)

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_InsertUser", con)
            { CommandType = CommandType.StoredProcedure };
            // Stored Procedure call

            cmd.Parameters.AddWithValue("@Name", vm.Name.Trim());
            cmd.Parameters.AddWithValue("@Email", vm.Email.Trim().ToLower());
            cmd.Parameters.AddWithValue("@Password", hashed);
            cmd.Parameters.AddWithValue("@Role", vm.Role ?? "User");

            con.Open();
            int newId = Convert.ToInt32(cmd.ExecuteScalar());
            // ExecuteScalar() -> Returns single value (newly inserted UserId)

            if (newId == -1)
                return (false, "Email already registered.");

            return (true, "Registration successful.");
        }

        // ── Login ──
        public (bool success, UserModel? user, string message) Login(LoginViewModel vm)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetUserByEmail", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Email", vm.Email.Trim().ToLower());
            con.Open();

            using var dr = cmd.ExecuteReader();
            // ExecuteReader() -> Used to read multiple rows (DataReader)

            if (!dr.Read())
                return (false, null, "Invalid email or password.");
            // No record found

            var user = new UserModel
            {
                UserId = Convert.ToInt32(dr["UserId"]),
                Name = dr["Name"].ToString()!,
                Email = dr["Email"].ToString()!,
                Password = dr["Password"].ToString()!,
                Role = dr["Role"].ToString()!,
                IsActive = Convert.ToBoolean(dr["IsActive"]),
                CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
            };

            if (!user.IsActive)
                return (false, null, "Your account has been deactivated.");

            bool valid = BCrypt.Net.BCrypt.Verify(vm.Password, user.Password);
            // Verify() -> Compares plain password with hashed password

            if (!valid)
                return (false, null, "Invalid email or password.");

            return (true, user, "Login successful.");
        }

        // ── Get All Users (Admin only) ──
        public List<UserModel> GetAllUsers()
        {
            var list = new List<UserModel>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetAllUsers", con)
            { CommandType = CommandType.StoredProcedure };

            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new UserModel
                {
                    UserId = Convert.ToInt32(dr["UserId"]),
                    Name = dr["Name"].ToString()!,
                    Email = dr["Email"].ToString()!,
                    Role = dr["Role"].ToString()!,
                    IsActive = Convert.ToBoolean(dr["IsActive"]),
                    CreatedDate = Convert.ToDateTime(dr["CreatedDate"])
                });
            }

            return list;
        }

        // ── Toggle Active ──
        public void ToggleActive(int userId, bool isActive)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ToggleUserActive", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            con.Open();
            cmd.ExecuteNonQuery();
            // ExecuteNonQuery() -> Used for INSERT, UPDATE, DELETE
        }
    }
}