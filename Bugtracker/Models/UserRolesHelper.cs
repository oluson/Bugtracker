
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;


namespace Bugtracker.Models
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleManager<IdentityRole> roleManager;

        public UserRolesHelper(ApplicationDbContext db)
        {
           this.manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db));
           this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(db));
            this.db = db;
        }

        public UserRolesHelper()
        {
        }

        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);
        }
        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool AddUserToRoles(string userId, string[] roleNames)
        {
            var result = manager.AddToRoles(userId, roleNames);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            
            return resultList;
        }
        public IList<string> ListAllRoles()
        {
            return roleManager.Roles.Where(r => r.Name != null).Select(r => r.Name).ToList();
        }

        public IList<string> ListAbsentUserRoles(string userId)
        {
            var roles = ListAllRoles();
            var AbsentUserRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!IsUserInRole(userId, role))
                {
                    AbsentUserRoles.Add(role);
                }
            }
            return AbsentUserRoles;
        }

        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            
            return resultList;
        }
    }
}