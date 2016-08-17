﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bugtracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Bugtracker.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ProjectRolesHelper pRA = new ProjectRolesHelper();

        [Authorize (Roles ="Admin,Project Manager")]
        public ActionResult Index()
        {
            // Populate Dropdown Lists
            //var context = new Models.ApplicationDbContext();

            var rolelist = db.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.Id, Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            var Projectslist = db.Project.OrderBy(r => r.Name).ToList().Select(rr =>
           new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name }).ToList();
            ViewBag.Projects = Projectslist;

            return View();
        }


        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {

            try
            {
                //var context = new Models.ApplicationDbContext();
                db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                db.SaveChanges();
                ViewBag.Message = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(string RoleName)
        {
            //var context = new Models.ApplicationDbContext();
            var thisRole = db.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            db.Roles.Remove(thisRole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            //var context = new Models.ApplicationDbContext();
            var thisRole = db.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                //var context = new Models.ApplicationDbContext();
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //  Adding Roles to a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            //var context = new Models.ApplicationDbContext();

            //if (context == null)
            //{
            //    throw new ArgumentNullException("context", "Context must not be null.");
            //}

            ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.AddToRole(user.Id, RoleName);


            ViewBag.Message = "Role created successfully !";

            // Repopulate Dropdown Lists
            var rolelist = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;
            var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            return View("Index");
        }


        //Getting a List of Roles for a User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
              //  var context = new Models.ApplicationDbContext();
                ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                ViewBag.RolesForThisUser = userManager.GetRoles(user.Id);


                // Repopulate Dropdown Lists
                var rolelist = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = rolelist;
                var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                ViewBag.Users = userlist;
                ViewBag.Message = "Roles retrieved successfully !";
            }

            return View("Index");
            
        }

        //Deleting a User from A Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            var account = new AccountController();
           // var context = new Models.ApplicationDbContext();
            ApplicationUser user = db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);


            if (userManager.IsInRole(user.Id, RoleName))
            {
                userManager.RemoveFromRole(user.Id, RoleName);
                ViewBag.Message = "User has been succesfully removed from this role !";
            }
            else
            {
                ViewBag.Message = "This user is currently not assigned to the selected role.";
            }

            // Repopulate Dropdown Lists
            var rolelist = db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;
            var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            return View("Index");
        }


        //Adding the user and project Lists 

        public ActionResult ProjectAssignment()
        {
            // Populate Dropdown Lists
            //var context = new Models.ApplicationDbContext();

            var Projectslist = db.Project.OrderBy(r => r.Name).ToList().Select(rr =>
            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Projects = Projectslist;

            var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            return View();
        }

        //  Adding users to Projects
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectAssignment(string UserName, string Name)
        {
           // var context = new Models.ApplicationDbContext();

            ProjectRolesHelper pRA = new ProjectRolesHelper();
            pRA.AddUserToProject(UserName, Name);


            ViewBag.Message = "User successfully added to project !";

            //Repopulate Dropdown Lists
            
            var userlist = db.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.Id, Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            var Projectslist = db.Project.OrderBy(r => r.Name).ToList().Select(rr =>
           new SelectListItem { Value = rr.Id.ToString(), Text = rr.Name }).ToList();
            ViewBag.Projects = Projectslist;


            return View("Index");
        }


    }
}