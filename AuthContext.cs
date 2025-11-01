using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSync
{
    internal class AuthContext
    {
        
            public static string Username { get; private set; }
            public static string Role { get; private set; }
            public static int UserId { get; private set; }

            public static void SignIn(int userId, string username, string role)
            {
                UserId = userId;
                Username = username;
                Role = role;
            }

            public static void SignOut()
            {
                UserId = 0;
                Username = null;
                Role = null;
            }

            public static bool IsInRole(string role) =>
                !string.IsNullOrEmpty(Role) && Role.Equals(role, StringComparison.OrdinalIgnoreCase);

            // Check module access
            public static bool CanAccess(string module)
            {
                if (IsInRole("Admin")) return true; // full access

                switch (Role)
                {
                    case "Accountant":
                        return module == "Fees";
                    case "Teacher":
                    case "tr":
                        return module == "Exams" || module == "Students" || module == "Attendance";
                    default:
                        return false;
                }
            }
        

    }
}
