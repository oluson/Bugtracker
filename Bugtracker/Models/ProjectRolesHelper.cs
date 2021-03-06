﻿using Bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Controllers
{
    public class ProjectRolesHelper
    {
        private ApplicationDbContext db;

        public ProjectRolesHelper(ApplicationDbContext context)
        {
            db = context;
        }

        public void AssignUser(string userId, int projectId)
        {
            if (!HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Project.Find(projectId);
                project.ProjectUsers.Add(user);
            }
        }

        public bool HasProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Project.Find(projectId);
            if (project.ProjectUsers.Contains(user)) //Or Any(u=> u.Id ==userId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveUser(string userId, int projectId)
        {
            if (HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Project.Find(projectId);
                project.ProjectUsers.Remove(user);
            }
        }

        public List<Projects> ListProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Project.ToList();
        }

        public List<ApplicationUser> ListUsers(int projectId)
        {
            var project = db.Project.Find(projectId);
            List<ApplicationUser> lst = new List<ApplicationUser>();
            
            foreach (ApplicationUser usr in project.ProjectUsers)
            {

                usr.DisplayName = usr.FirstName + " " + usr.LastName;
                lst.Add(usr);
            }
            return lst;
        }

        public List<ApplicationUser> ListAbsentUsers(int projectId)
        {
            var project = db.Project.Find(projectId);
            var users = db.Users.ToList();
            var absentUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (!HasProject(user.Id, projectId))
                {
                    user.DisplayName = user.FirstName + " " + user.LastName;
                    absentUsers.Add(user);
                }
            }
            return absentUsers;
        }

        public List<string> ListProjectManagers(int projectId)
        {
            var projectManagers = new List<string>();
            var project = db.Project.Find(projectId);
            var projectUsers = project.ProjectUsers.ToList();
            UserRolesHelper helper = new UserRolesHelper(db);
            foreach (var user in projectUsers)
            {
                if (helper.IsUserInRole(user.Id, "Project Manager"))
                {
                    projectManagers.Add(user.Email);
                }
            }
            return projectManagers;
        }

    }
}