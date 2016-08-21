using Bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugtracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // private UserRolesHelper Urh = new UserRolesHelper(new ApplicationDbContext);
        public ActionResult Index()
        {
            DashboardViewModel mo = new DashboardViewModel();
            mo.Name = "Bugtracker";

            return View(mo);
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