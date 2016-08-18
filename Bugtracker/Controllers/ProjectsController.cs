﻿using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Bugtracker.Models;
using Microsoft.AspNet.Identity;

namespace Bugtracker.Controllers
{
    //[RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
         

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var projects = new List<Projects>();
            ProjectRolesHelper helper = new ProjectRolesHelper(db);
            projects = helper.ListProjects(userId);

            return View(projects.Where(p => p.Archived == false).OrderBy(p => p.Deadline));
        }

        //GET: Projects/ViewAll
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ViewAll()
        {
            var projects = db.Project.Where(p => p.Archived == false).OrderBy(p => p.Deadline).ToList();
            return View(projects);
        }

        //GET: Projects/Archive
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Archive()
        {
            var projects = db.Project.Where(p => p.Archived == true).OrderBy(p => p.Deadline).ToList();
            return View(projects);
        }

        // GET: Projects/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            UserRolesHelper userHelper = new UserRolesHelper(db);
            ProjectRolesHelper projectHelper = new ProjectRolesHelper(db);
            var userRoles = userHelper.ListUserRoles(userId);
            var tickets = new List<Tickets>();
            if (userRoles.Contains("Admin") || (userRoles.Contains("Project Manager")))
            {
                tickets = project.Tickets.ToList();
            }
            else if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                tickets = project.Tickets.Where(t => t.AssignedToUserId == userId || t.OwnerUserId == userId).ToList();
            }
            else if (userRoles.Contains("Developer"))
            {
                tickets = project.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
            }
            else if (userRoles.Contains("Submitter"))
            {
                tickets = project.Tickets.Where(t => t.OwnerUserId == userId).ToList();
            }

            ViewBag.TicketList = tickets;
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Deadline")] Projects project)
        {
            if (ModelState.IsValid)
            {
                ProjectRolesHelper helper = new ProjectRolesHelper(db);
                project.Created = System.DateTimeOffset.Now;
                project.Archived = false;
                var userId = User.Identity.GetUserId();
                db.Project.Add(project);
                helper.AssignUser(userId, project.Id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Deadline")] Projects project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        //GET: Project/AssignUsers
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AssignUsers(int? id)
        {
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Projects project = db.Project.Find(id);
                if (project == null)
                {
                    return HttpNotFound();
                }

                AssignProjectUsersViewModel model = new AssignProjectUsersViewModel();
                ProjectRolesHelper helper = new ProjectRolesHelper(db);
             
                model.ProjectId = project.Id;
                model.ProjectTitle = project.Name;
                var currentUsers = helper.ListUsers(model.ProjectId);
                model.UsersList = currentUsers;
                model.CurrentUsers = new SelectList(currentUsers, "Id", "LastName");
                var absentUsers = helper.ListAbsentUsers(model.ProjectId);
                model.AbsentUsers = new SelectList(absentUsers, "Id", "LastName");
                return View(model);
            }
        }

        ////POST: Project/AddUser
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(string AddUserId, int ProjectId)
        {
            ProjectRolesHelper helper = new ProjectRolesHelper(db);
            helper.AssignUser(AddUserId, ProjectId);
            db.SaveChanges();
            return RedirectToAction("AssignUsers", new { id = ProjectId });
        }

        ////POST: Project/RemoveUser
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveUser(string RemoveUserId, int ProjectId)
        {
            ProjectRolesHelper helper = new ProjectRolesHelper(db);
            var tickets = db.Tickets.Where(t => t.AssignedToUserId == RemoveUserId && t.ProjectId == ProjectId).ToList();
            foreach (var ticket in tickets)
            {
                ticket.AssignedToUserId = null;
                ticket.TicketStatus.Name = "Unassigned";
                ticket.Updated = System.DateTimeOffset.Now;
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Modified").IsModified = true;
                db.Entry(ticket).Property("AssigneeId").IsModified = true;
                db.Entry(ticket).Property("Status").IsModified = true;
                db.SaveChanges();
            }
            helper.RemoveUser(RemoveUserId, ProjectId);
            db.SaveChanges();
            return RedirectToAction("AssignUsers", new { id = ProjectId });
        }

        // GET: Projects/ArchiveProject/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ArchiveProject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/ArchiveProject
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArchiveProject(int id)
        {
            Projects project = db.Project.Find(id);
            project.Archived = true;
            db.Project.Attach(project);
            db.Entry(project).Property("Archived").IsModified = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}