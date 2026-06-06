using InvoiceGenerator.Models;

namespace InvoiceGenerator.Helper
{
    public static class SessionHelper
    {
        private const string KEY_ID = "UserId";
        private const string KEY_NAME = "UserName";
        private const string KEY_EMAIL = "UserEmail";
        private const string KEY_ROLE = "UserRole";
        private const string KEY_DEPARTMENT = "UserDepartment";
        private const string KEY_DESIGNATION = "UserDesignation";

        // ── Role Groups ──────────────────────────────────────────
        // Full Access  : Admin, MD, CEO, HOD
        // Invoice Only : User, Supervisor
        // No Access    : Operator
        // ─────────────────────────────────────────────────────────

        private static readonly string[] FullAccessRoles =
            { "Admin", "MD", "CEO", "HOD" };

        private static readonly string[] InvoiceAccessRoles =
            { "Admin", "MD", "CEO", "HOD", "Supervisor" };

        // ── Set / Clear ──────────────────────────────────────────
        public static void SetUser(ISession session, UserModel user)
        {
            session.SetInt32(KEY_ID, user.UserId);
            session.SetString(KEY_NAME, user.Name);
            session.SetString(KEY_EMAIL, user.Email);
            session.SetString(KEY_ROLE, user.Role);
            session.SetString(KEY_DEPARTMENT, user.Department ?? "");
            session.SetString(KEY_DESIGNATION, user.Designation ?? "");
        }

        public static void Clear(ISession session)
            => session.Clear();

        // ── Login check ──────────────────────────────────────────
        public static bool IsLoggedIn(ISession session)
            => session.GetInt32(KEY_ID).HasValue;

        // ── Getters ──────────────────────────────────────────────
        public static int GetUserId(ISession session)
            => session.GetInt32(KEY_ID) ?? 0;

        public static string GetUserName(ISession session)
            => session.GetString(KEY_NAME) ?? "";

        public static string GetUserEmail(ISession session)
            => session.GetString(KEY_EMAIL) ?? "";

        public static string GetUserRole(ISession session)
            => session.GetString(KEY_ROLE) ?? "";

        public static string GetUserDepartment(ISession session)
            => session.GetString(KEY_DEPARTMENT) ?? "";

        public static string GetUserDesignation(ISession session)
            => session.GetString(KEY_DESIGNATION) ?? "";

        // ── Role checks ──────────────────────────────────────────

        /// Admin, MD, CEO, HOD — full access to masters + invoice
        public static bool IsAdmin(ISession session)
            => FullAccessRoles.Contains(GetUserRole(session));

        /// Admin, MD, CEO, HOD, User, Supervisor — invoice access
        public static bool HasInvoiceAccess(ISession session)
            => InvoiceAccessRoles.Contains(GetUserRole(session));

        /// Operator — blocked from everything
        public static bool IsOperator(ISession session)
            => GetUserRole(session) == "Operator";

        /// Convenience: is the role one of the named executive roles
        public static bool IsMD(ISession session)
            => GetUserRole(session) == "MD";

        public static bool IsCEO(ISession session)
            => GetUserRole(session) == "CEO";

        public static bool IsHOD(ISession session)
            => GetUserRole(session) == "HOD";

        public static bool IsSupervisor(ISession session)
            => GetUserRole(session) == "Supervisor";
    }
}