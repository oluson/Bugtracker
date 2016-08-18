﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bugtracker.Models;
using Microsoft.AspNet.Identity;
using Bugtracker.Controllers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Bugtracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            TicketsHelper helper = new TicketsHelper(db);
            var tickets = helper.GetUserTickets(userId);

            return View(tickets);
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                ViewBag.UserId = User.Identity.GetUserId();
                return View(ticket);
            }
            TempData["Error"] = "Sorry, you do not have permission to view that ticket.";
            return RedirectToAction("Index");
        }

        // GET: Tickets/History/5
        [Authorize]
        public ActionResult History(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                ViewBag.UserId = User.Identity.GetUserId();
                return View(ticket);
            }
            TempData["Error"] = "Sorry, you do not have permission to view that ticket.";
            return RedirectToAction("Index");
        }

        [Authorize]
        // GET: Tickets/Create
        public ActionResult Create(int? id)
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
            ViewBag.ProjectId = id;
            ViewBag.ProjectTitle = project.Title;
            return View();
        }
        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProjectId,Title,Body,Priority,Type")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.Title == null)
                {
                    ticket.Title = StringUtilities.Shorten(ticket.Description, 50);
                }
                ticket.Created = System.DateTimeOffset.Now;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.TicketStatus.Name = "Unassigned";
                TicketHistories history = new TicketHistories();
                history.Updated = DateTime.Now;
                var historyBody = "Ticket created. <br> Title: " + ticket.Title + "<br> Body: " + ticket.Description + "<br>" + "Priority: " + ticket.TicketPriority + ", Type: " + ticket.TicketType.Name + ", Status: " + ticket.TicketStatus.Name;
                history.Ticket.Description = historyBody;
                history.TicketId = ticket.Id;
                db.Tickets.Add(ticket);
                db.TicketHistory.Add(history);
                db.SaveChanges();

                var user = db.Users.Find(ticket.OwnerUserId);
                //Send email to project managers
                ProjectRolesHelper helper = new ProjectRolesHelper(db);
                var projectManagers = helper.ListProjectManagers(ticket.ProjectId);
                foreach (var pm in projectManagers)
                {
                    var svc = new EmailService();
                    var msg = new IdentityMessage();
                    msg.Destination = pm;
                    msg.Subject = "Bug Tracker: Ticket Created";
                    msg.Body = user.FirstName + " " + user.LastName + " has created the ticket '" + ticket.Title + "'. To assign this ticket to a user, please visit https://epalmer-bugtracker.azurewebsites.net/Tickets/Details/" + ticket.Id;
                    await svc.SendAsync(msg);
                }

                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        //Send Email to Ticket's Assigned Developer
        public async Task<bool> NotifyDeveloper(int ticketId, string editorId, string assigneeId)
        {
            var user = db.Users.Find(editorId);
            var ticket = db.Tickets.Find(ticketId);
            var developer = db.Users.Find(assigneeId);
            if (user != null && ticket != null && developer != null)
            {
                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = developer.Email;
                msg.Subject = "Bug Tracker: Ticket Modified";
                msg.Body = user.FirstName + " " + user.LastName + " has modified the ticket '" + ticket.Title + "'. To view this ticket, please visit https://epalmer-bugtracker.azurewebsites.net/Tickets/Details/" + ticket.Id + " If you have any questions, " + user.FirstName + " " + user.LastName + " can be contacted at " + user.Email;
                await svc.SendAsync(msg);
            }

            return true;
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (!ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                TempData["Error"] = "Sorry, you do not have permission to access that ticket.";
                return RedirectToAction("Index");
            }
            var project = db.Project.FirstOrDefault(p => p.Id == ticket.ProjectId);
            var ProjectTitle = project.Title;
            var type = db.TicketType.Find(ticket.TicketTypeId);
            var TicketType = type.Name;
            ViewBag.ProjectTitle = ProjectTitle;
            ViewBag.TicketType = TicketType;
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Created,Title,Body,Priority,Type, AssigneeId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket.Title == null)
                {
                    ticket.Title = StringUtilities.Shorten(ticket.Description, 50);
                }
                ticket.Updated = System.DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                if (oldTicket.Title != ticket.Title || oldTicket.Description != ticket.Description || oldTicket.TicketType.Name != ticket.TicketType.Name || oldTicket.TicketPriority != ticket.TicketPriority)
                {
                    TicketHistories history = new TicketHistories();
                    history.Updated = DateTime.Now;
                    StringBuilder historyBody = new StringBuilder();
                    historyBody.Append("Ticket edited by ");
                    historyBody.Append(user.FirstName + " " + user.LastName);
                    if (oldTicket.Title != ticket.Title)
                    {
                        historyBody.AppendFormat("<br>Old Title: {0} <br> New Title: {1}", oldTicket.Title, ticket.Title);
                    }
                    if (oldTicket.Description != ticket.Description)
                    {
                        historyBody.AppendFormat("<br>Old Body Content: {0} <br> New Body Content: {1}", oldTicket.Description, ticket.Description);
                    }
                    if (oldTicket.TicketType.Name != ticket.TicketType.Name)
                    {
                        historyBody.AppendFormat("<br>Old Type: {0} <br> New Type: {1}", oldTicket.TicketType.Name, ticket.TicketType.Name);
                    }
                    if (oldTicket.TicketPriority != ticket.TicketPriority)
                    {
                        historyBody.AppendFormat("<br>Old Priority: {0} <br> New Priority: {1}", oldTicket.TicketPriority, ticket.TicketPriority);
                    }
                    history.Ticket.Description = historyBody.ToString();
                    history.TicketId = ticket.Id;
                    db.TicketHistory.Add(history);
                }
                else
                {
                    ModelState.AddModelError("", "Error: No changes have been made.");
                    return View(ticket);
                }
                db.Tickets.Attach(ticket);
                //db.Entry(ticket).State = EntityState.Modified;
                db.Entry(ticket).Property("Modified").IsModified = true;
                db.Entry(ticket).Property("Title").IsModified = true;
                db.Entry(ticket).Property("Body").IsModified = true;
                db.Entry(ticket).Property("Priority").IsModified = true;
                db.SaveChanges();

                await NotifyDeveloper(ticket.Id, userId, ticket.AssignedToUserId);
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            return View(ticket);
        }

        //GET Tickets/Assign Users
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            AssignTicketUserViewModel AssignModel = new AssignTicketUserViewModel();
            AssignModel.TicketId = ticket.Id;
            AssignModel.TicketTitle = ticket.Title;
            ProjectRolesHelper helper = new ProjectRolesHelper(db);
            UserRolesHelper userHelper = new UserRolesHelper(db);
            var projectUsers = helper.ListUsers(ticket.ProjectId);
            var projectDevelopers = new List<ApplicationUser>();
            foreach (var user in projectUsers)
            {
                if (userHelper.IsUserInRole(user.Id, "Developer"))
                {
                    projectDevelopers.Add(user);
                }
            }
            if (ticket.AssignedToUser != null)
            {
                AssignModel.TicketAssignedTo = ticket.AssignedToUser.FirstName + " " + ticket.AssignedToUser.LastName;
            }
            AssignModel.UsersList = new SelectList(projectDevelopers, "Id", "LastName");
            return View(AssignModel);
        }

        ////POST: Tickets/AssignUser
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignUser(string UserId, int TicketId)
        {
            //Ticket details
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == TicketId);
            var ticket = db.Tickets.Find(TicketId);
            ticket.AssignedToUserId = UserId;
            ticket.TicketStatus.Name = "Assigned";
            ticket.Updated = System.DateTimeOffset.Now;
            //Add to ticket history
            TicketHistories history = new TicketHistories();
            history.Updated = System.DateTime.Now;
            var user = db.Users.Find(UserId);
            var historyBody = "Ticket assigned to " + user.FirstName + " " + user.LastName + ". Ticket now Active.";
            history.Ticket.Description = historyBody;
            history.TicketId = ticket.Id;
            db.TicketHistory.Add(history);
            //send email to previous developer
            if (oldTicket.AssignedToUserId != ticket.AssignedToUserId && oldTicket.AssignedToUserId == null)
            {
                var svc2 = new EmailService();
                var msg2 = new IdentityMessage();
                msg2.Destination = user.Email;
                msg2.Subject = "Bug Tracker: Ticket Reassigned";
                msg2.Body = ticket.OwnerUser.FirstName + " " + ticket.OwnerUser.LastName + " has reassigned the ticket '" + ticket.Title + "' to another developer. You are no longer responsible for this ticket. If you have any questions regarding this ticket, " + ticket.OwnerUser.FirstName + " can be contacted at " + ticket.OwnerUser.Email;
                await svc2.SendAsync(msg2);
            }
            if (oldTicket.AssignedToUserId == ticket.AssignedToUserId)
            {
                ModelState.AddModelError("", "Error: No changes have been made.");
                return RedirectToAction("AssignUser", new { id = TicketId });
            }
            //Save to database
            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property("AssigneeId").IsModified = true;
            db.Entry(ticket).Property("Status").IsModified = true;
            db.Entry(ticket).Property("Modified").IsModified = true;
            db.SaveChanges();
            //Send email to developer assigned
            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Destination = user.Email;
            msg.Subject = "Bug Tracker: New Ticket Assigned";
            msg.Body = ticket.OwnerUser.FirstName + " " + ticket.OwnerUser.LastName + " has assigned you the ticket '" + ticket.Title + "'. To view this ticket, please visit https://epalmer-bugtracker.azurewebsites.net/Tickets/Details/" + ticket.Id + " If you have any questions regarding this ticket, " + ticket.OwnerUser.FirstName + " " + ticket.OwnerUser.LastName + " can be contacted at " + ticket.OwnerUser.Email;
            await svc.SendAsync(msg);

            return RedirectToAction("Details", new { id = TicketId });
        }

        // GET: Tickets/Close/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Close
        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Close(int id)
        {
            Tickets ticket = db.Tickets.Find(id);
            ticket.TicketStatus.Name = "Closed";
            ticket.Updated = System.DateTimeOffset.Now;
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            TicketHistories history = new TicketHistories();
            history.Updated = System.DateTime.Now;
            var historyBody = "Ticket Closed by " + user.FirstName + " " + user.LastName;
            history.Ticket.Description = historyBody;
            history.TicketId = ticket.Id;
            db.TicketHistory.Add(history);
            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property("Status").IsModified = true;
            db.Entry(ticket).Property("Modified").IsModified = true;
            db.SaveChanges();

            await NotifyDeveloper(id, userId, ticket.AssignedToUserId);
            return RedirectToAction("Index");
        }

        // GET: Tickets/Resolve/5
        [Authorize(Roles = "Developer")]
        public ActionResult Resolve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            if (ticket.AssignedToUserId == userId)
            {
                ViewBag.UserId = User.Identity.GetUserId();
                return View(ticket);
            }
            TempData["Error"] = "Sorry, you do not have permission to view that ticket.";
            return RedirectToAction("Index");
        }

        // POST: Tickets/Resolve
        [Authorize(Roles = "Developer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Resolve(int id)
        {
            //Ticket Information
            Tickets ticket = db.Tickets.Find(id);
            ticket.TicketStatus.Name = "Resolved";
            ticket.Updated = System.DateTimeOffset.Now;
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            //Add to ticket history
            TicketHistories history = new TicketHistories();
            history.Updated = System.DateTime.Now;
            var historyBody = "Ticket marked as Resolved by " + user.FirstName + " " + user.LastName;
            history.Ticket.Description = historyBody;
            history.TicketId = ticket.Id;
            db.TicketHistory.Add(history);
            //Save ticket changes to database
            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property("Status").IsModified = true;
            db.Entry(ticket).Property("Modified").IsModified = true;
            db.SaveChanges();
            //Send email to project managers
            ProjectRolesHelper helper = new ProjectRolesHelper(db);
            var projectManagers = helper.ListProjectManagers(ticket.ProjectId);
            foreach (var pm in projectManagers)
            {
                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = pm;
                msg.Subject = "Bug Tracker: Ticket Resolved";
                msg.Body = user.FirstName + " " + user.LastName + " has marked the ticket '" + ticket.Title + "' as Resolved. To close this ticket, please visit https://epalmer-bugtracker.azurewebsites.net/Tickets/Details/" + ticket.Id + " If there are still issues left to resolve on the ticket, the ticket's developer can be reached at " + ticket.AssignedToUser.Email;
                await svc.SendAsync(msg);
            }

            return RedirectToAction("Details", new { id = ticket.Id });
        }

        //GET: Tickets/AddComment
        [Authorize]
        public ActionResult AddComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (!ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                TempData["Error"] = "Sorry, you do not have permission to access that ticket.";
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = id;
            ViewBag.TicketTitle = ticket.Title;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddComment([Bind(Include = "Id,TicketId,Content")] TicketComments comment)
        {
            if (ModelState.IsValid)
            {
                comment.UserId = User.Identity.GetUserId();
                comment.Created = System.DateTime.Now;
                db.TicketComment.Add(comment);
                db.SaveChanges();

                var ticket = db.Tickets.Find(comment.TicketId);
                var assigneeId = ticket.AssignedToUserId;
                await NotifyDeveloper(comment.TicketId, comment.UserId, assigneeId);

                return RedirectToAction("Details", new { id = comment.TicketId });
            }
            return View();
        }

        // POST: Delete Comment
        [Authorize]
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            TicketComments comment = db.TicketComment.Find(id);
            db.TicketComment.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = comment.TicketId });
        }

        //GET: Tickets/AddAttachment
        [Authorize]
        public ActionResult AddAttachment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (!ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                TempData["Error"] = "Sorry, you do not have permission to access that ticket.";
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = id;
            ViewBag.TicketTitle = ticket.Title;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAttachment([Bind(Include = "Id,TicketId,Description,MediaURL")] TicketAttachments attachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendly(image))
                {
                    if (ImageUploadValidator.IsImage(image) || image.FileName.Contains(".pdf") || image.FileName.Contains(".doc"))
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var uniqueId = DateTime.Now.Ticks;
                        fileName = Regex.Replace(fileName, @"[!@#$%_\s]", "");
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), uniqueId + fileName));
                        attachment.FilePath = "/Uploads/" + uniqueId + fileName;
                    }
                    else
                    {
                        return View();
                    }
                }
                attachment.UserId = User.Identity.GetUserId();
                db.TicketAttachment.Add(attachment);
                db.SaveChanges();

                var ticket = db.Tickets.Find(attachment.TicketId);
                await NotifyDeveloper(attachment.TicketId, attachment.UserId, ticket.AssignedToUserId);

                return RedirectToAction("Details", new { id = attachment.TicketId });
            }
            return View();
        }

        // POST: Delete Attachment
        [Authorize]
        [HttpPost, ActionName("DeleteAttachment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttachment(int id)
        {
            TicketAttachments attachment = db.TicketAttachment.Find(id);
            db.TicketAttachment.Remove(attachment);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = attachment.TicketId });
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