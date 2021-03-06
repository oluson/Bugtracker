﻿using Bugtracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Principal;

namespace Bugtracker.Controllers
{
    public static class HelperExtension
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static RoleManager<IdentityRole> roleManager;

        public static string GetDisplayName (this IIdentity user)
        {
            return db.Users.Find(user.GetUserId()).DisplayName;
        }
        public static IList<string> GetUserRole(this IIdentity users)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            return helper.ListUserRoles(users.GetUserId());
           // return db.Roles.Find(Roles.GetRolesForUser());
        }
    }
}