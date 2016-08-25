using Bugtracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Bugtracker.Controllers
{
    public static class HelperExtension
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetDisplayName (this IIdentity user)
        {
            return db.Users.Find(user.GetUserId()).DisplayName;
        }
    }
}