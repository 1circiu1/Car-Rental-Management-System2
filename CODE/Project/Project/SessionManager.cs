using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Models;

namespace Project
{
    public static class SessionManager
    {
        public static User CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;
        public static bool isAdmin => IsLoggedIn && CurrentUser.Role == "Admin";
    }
}
