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
        public bool IsUserOnProject(string userId, int ProjectId)
        {
            var project = db.Project.Find(ProjectId);
            var projectUser = project.ProjectUsers.Any(u => u.Id.Equals(userId));
            return projectUser;
        }
        public List<Projects> ListUserProjects(string userId)
        {
            ApplicationUser users = db.Users.Find(userId);
            return users.Project.ToList();

        }
        public void AddUserToProject(string userId, string ProjectId)
        {
            ApplicationUser users = db.Users.Find(userId);
            Projects project = db.Project.Find(ProjectId);
            project.ProjectUsers.Add(users);
            db.SaveChanges();
        }
        //public bool AddUserToRoles(string userId, string[] ProjectId)
        //{
        //    var result = manager.AddToRoles(userId, ProjectId);
        //    return result.Succeeded;
        public void RemoveUserFromProject(string userId, string ProjectId)
        {
            ApplicationUser users = db.Users.Find(userId);
            Projects project = db.Project.Find(ProjectId);
            project.ProjectUsers.Remove(users);
            db.SaveChanges();
        }
        public List<ApplicationUser> ListProjectUsers(int ProjectId)
        {
            Projects project = db.Project.Find(ProjectId);
            return project.ProjectUsers.ToList();
        }
        public List<ApplicationUser> UsersNotInProject(int ProjectId)
        {
            Projects projects = db.Project.Find(ProjectId);
            var projUsers = projects.ProjectUsers;
            return projects.ProjectUsers.Where(u => !projUsers.Contains(u)).ToList();
        }
    }
}