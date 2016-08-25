using Bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Bugtracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // private UserRolesHelper Urh = new UserRolesHelper(new ApplicationDbContext);
        public ActionResult Index()
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

            return View("Dashboard", model);
        }

        public ActionResult About(string uid, string role)
        {
            //UserRolesHelper helper = new UserRolesHelper();
            //helper.AddUserToRole(uid, role);


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        // GET: UserProfile
        public ActionResult UserProfile(string UserId, int ProjectId)
        {
            var user = db.Users.Find(UserId);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserProfileView profile = new UserProfileView();
            UserRolesHelper helper = new UserRolesHelper();
            ProjectRolesHelper phelper = new ProjectRolesHelper(db);
            TicketsHelper thelper = new TicketsHelper(db);

            profile.Name = user.FirstName + " " + user.LastName;
            profile.Email = user.Email;
            profile.PhoneNumber = user.PhoneNumber;
            profile.Roles = helper.ListUserRoles(user.Id);
            profile.ProjectCount = phelper.ListProjects(UserId).ToList().Count;
            profile.TicketsSubmitted = thelper.GetUserTickets(UserId).ToList().Count;
            List<Tickets> list = thelper.GetUserTickets(UserId).ToList();
            int assigned = 0;
            int resolved = 0;
            foreach (Tickets tick in list)
            {
                if (tick.AssignedToUserId == UserId)
                {
                    assigned++;
                }

                if (tick.TicketStatusId == 3)
                {
                    resolved++;
                }
            }
            profile.TicketsAssigned = assigned;
            profile.TicketsResolved = resolved;
            profile.ProjectId = ProjectId;
            return View(profile);
        }      
    }
}