using InvoiceGenerator.Models;

namespace InvoiceGenerator.Helper
{
    public static class SessionHelper
    {
        // Keys used to store data in session
        private const string KEY_ID = "UserId";
        private const string KEY_NAME = "UserName";
        private const string KEY_EMAIL = "UserEmail";
        private const string KEY_ROLE = "UserRole";

        public static void SetUser(ISession session, UserModel user)
        // ISession -> Interface used to store and retrieve user-specific data across HTTP requests
        {
            session.SetInt32(KEY_ID, user.UserId);
            // SetInt32() -> Stores integer value in session

            session.SetString(KEY_NAME, user.Name);
            // SetString() -> Stores string value in session

            session.SetString(KEY_EMAIL, user.Email);

            session.SetString(KEY_ROLE, user.Role);
        }

        public static void Clear(ISession session)
            => session.Clear();
        // Clear() -> Removes all data from the current session (logout scenario)

        public static bool IsLoggedIn(ISession session)
            => session.GetInt32(KEY_ID).HasValue;
        // GetInt32() -> Retrieves integer value from session (returns int?)
        // HasValue -> Checks if userId exists (i.e., user is logged in)

        public static int GetUserId(ISession session)
            => session.GetInt32(KEY_ID) ?? 0;
        // ?? (null-coalescing operator) -> Returns 0 if session value is null

        public static string GetUserName(ISession session)
            => session.GetString(KEY_NAME) ?? "";
        // GetString() -> Retrieves string value from session

        public static string GetUserRole(ISession session)
            => session.GetString(KEY_ROLE) ?? "";

        public static bool IsAdmin(ISession session)
            => GetUserRole(session) == "Admin";
        // Checks if logged-in user has Admin role
    }
}