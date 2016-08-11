using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class ProjectRolesHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();
        public bool IsUserInRole(string userId, string ProjectName)
        {
            return manager.IsInRole(userId, ProjectName);
        }
        public ICollection<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }
        public bool AddUserToRole(string userId, string ProjectName)
        {
            var result = manager.AddToRole(userId, ProjectName);
            return result.Succeeded;
        }
        public bool AddUserToRoles(string userId, string[] ProjectName)
        {
            var result = manager.AddToRoles(userId, ProjectName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string ProjectName)
        {
            var result = manager.RemoveFromRole(userId, ProjectName);
            return result.Succeeded;
        }
        public ICollection<ApplicationUser> UsersInRole(string ProjectName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, ProjectName))
                    resultList.Add(user);
            }
            return resultList;
        }
        public ICollection<ApplicationUser> UsersNotInRole(string ProjectName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, ProjectName))
                    resultList.Add(user);
            }
            return resultList;
        }
    }
}