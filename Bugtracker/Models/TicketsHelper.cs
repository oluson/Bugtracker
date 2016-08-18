using Bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bugtracker.Controllers
{
    public class TicketsHelper
    {
        private ApplicationDbContext db;

        public TicketsHelper(ApplicationDbContext context)
        {
            db = context;
        }

        public List<Tickets> GetUserTickets(string userId)
        {

            var user = db.Users.Find(userId);
            UserRolesHelper userHelper = new UserRolesHelper(db);
            ProjectRolesHelper projectHelper = new ProjectRolesHelper(db);
            var userRoles = userHelper.ListUserRoles(userId);
            var tickets = new List<Tickets>();
            if (userRoles.Contains("Admin"))
            {
                tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).ToList();
            }
            else if (userRoles.Contains("Project Manager"))
            {
                tickets = user.Project.SelectMany(p => p.Tickets).ToList();
            }
            else if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                tickets = db.Tickets.Where(t => t.AssignedToUserId == userId || t.OwnerUserId == userId).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).ToList();

            }
            else if (userRoles.Contains("Developer"))
            {
                //tickets where AssigneedId == userId
                tickets = db.Tickets.Where(t => t.AssignedToUserId == userId).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).ToList();
            }
            else if (userRoles.Contains("Submitter"))
            {
                //tickets where OwnerId == userID
                tickets = db.Tickets.Where(t => t.OwnerUserId == userId).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).ToList();
            }
            return tickets;
        }

        public bool HasTicketPermission(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            UserRolesHelper helper = new UserRolesHelper(db);
            var userRoles = helper.ListUserRoles(userId);
            if (userRoles.Contains("Admin"))
            {
                return true;
            }
            else if (userRoles.Contains("Project Manager") && user.Project.SelectMany(p => p.Tickets).ToList().Contains(ticket))
            {
                return true;
            }
            else if (userRoles.Contains("Submitter") && userRoles.Contains("Developer"))
            {
                if (ticket.AssignedToUserId == userId || ticket.OwnerUserId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (userRoles.Contains("Developer") && ticket.AssignedToUserId == userId)
            {
                return true;
            }
            else if (userRoles.Contains("Submitter") && ticket.OwnerUserId == userId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}



