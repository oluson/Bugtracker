using Bugtracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugtracker.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            SetDashboard();
            return RedirectToAction("ListUsers");
        }


        private void SetDashboard()
        {
            var userId = User.Identity.GetUserId();
            var tickets = new List<Tickets>();
            var attachments = new List<TicketAttachments>();
            var comments = new List<TicketComments>();
            int projects = 0;

            if (User.IsInRole("Admin"))
            {
                tickets = db.Tickets.ToList();
                attachments = db.TicketAttachment.Take(5).ToList();
                projects = db.Project.Count();
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComment)
                        comments.Add(comment);
            }
            else if (User.IsInRole("Project Manager"))
            {
                tickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachment)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComment)
                        comments.Add(comment);
                ProjectRolesHelper helper = new ProjectRolesHelper(db);
                projects = helper.ListProjects(userId).Count();
            }
            else if (User.IsInRole("Developer") && User.IsInRole("Project Manager"))
            {
                tickets = db.Tickets.Where(t => t.Project.ProjectUsers.Contains(db.Users.Find(userId))).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachment)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComment)
                        comments.Add(comment);

                ProjectRolesHelper helper = new ProjectRolesHelper(db);
                projects = helper.ListProjects(userId).Count();
            }
            else if (User.IsInRole("Developer"))
            {
                tickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.TicketAttachment)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.TicketComment)
                        comments.Add(comment);

                ProjectRolesHelper helper = new ProjectRolesHelper(db);
                projects = helper.ListProjects(userId).Count();
            }

            var model = new DashboardViewModel()
            {
                Tickets = tickets,
                Attachments = attachments,
                Comments = comments.Take(5),
                ProjectsAmt = projects
            };

            ViewBag.DashboardModel = model;
            //return View("Dashboard", model);
        }
        [Authorize(Roles = "Admin")]
        //
        // GET: /Admin/ListUsers
        public ActionResult ListUsers()
        {
            UsersListViewModel UserModel = new UsersListViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var users = db.Users.ToList();
            UserModel.Users = new List<UsersViewModel>();
            foreach (var user in users)
            {
                var id = user.Id;
                var name = user.FirstName + " " + user.LastName;
                IList<string> roles = helper.ListUserRoles(id);
                var projects = new List<string>();
                foreach (var project in user.Project)
                {
                    projects.Add(project.Title);
                }
                UserModel.Users.Add(new UsersViewModel(id, name, roles, projects));
            }
            SetDashboard();
            return View(UserModel);
        }

        //
        // GET: /Admin/EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminViewModel AdminModel = new AdminViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            AdminModel.AbsentRoles = new MultiSelectList(absentRoles);
            AdminModel.Roles = new MultiSelectList(currentRoles);
            AdminModel.Id = user.Id;
            AdminModel.Name = user.FirstName + " " + user.LastName;

            SetDashboard();
            return View(AdminModel);
        }

        //
        // POST: Add User Role
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public ActionResult AddRole(string Id, List<string> SelectedAbsentRoles)
        //{
        //    SetDashboard();
        //    if (ModelState.IsValid)
        //    {
        //        UserRolesHelper helper = new UserRolesHelper(db);
        //        var user = db.Users.Find(Id);
        //        foreach (var role in SelectedAbsentRoles)
        //        {
        //            helper.AddUserToRole(Id, role);
        //        }

        //        db.Entry(user).State = EntityState.Modified;
        //        db.Users.Attach(user);
        //        db.SaveChanges();
        //        return RedirectToAction("ListUsers");
        //    }
        //    return View(Id);
        //}

        //
        // POST: Add User Role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddRole(string userId, string role)
        {
            SetDashboard();
            if (ModelState.IsValid)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                var user = db.Users.Find(userId);
                foreach (var r in helper.ListAllRoles())
                {
                    if (r.ToString() == role)
                    {
                        helper.AddUserToRole(userId, r);
                    }
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("ListUsers");
            }
            return View();
        }

        //
        // POST: Remove User Role
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public ActionResult RemoveRole(string Id, List<string> SelectedCurrentRoles)
        //{
        //    SetDashboard();
        //    if (ModelState.IsValid)
        //    {
        //        UserRolesHelper helper = new UserRolesHelper(db);
        //        var user = db.Users.Find(Id);
        //        foreach (var role in SelectedCurrentRoles)
        //        {
        //            helper.RemoveUserFromRole(Id, role);
        //        }

        //        db.Entry(user).State = EntityState.Modified;
        //        db.Users.Attach(user);
        //        db.SaveChanges();
        //        return RedirectToAction("ListUsers");
        //    }
        //    return View(Id);
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult RemoveRole(string userId, string role)
        {
            SetDashboard();
            if (ModelState.IsValid)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                var user = db.Users.Find(userId);
                foreach (var r in helper.ListAllRoles())
                {
                    if (r.ToString() == role)
                    {
                        helper.RemoveUserFromRole(userId, r);
                    }
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("ListUsers");
            }
            return View();
        }
    }
}
