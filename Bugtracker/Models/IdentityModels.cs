using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Collections;

namespace Bugtracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {      
        public string FirstName { get; set; }
            
        public string LastName { get; set; }
   
        public string DisplayName { get; set; }

        public ApplicationUser()
        { 
          
            Project = new HashSet<Projects>();
            TicketAttachment = new HashSet<TicketAttachments>();
            TicketComment = new HashSet<TicketComments>();
            TicketHistory = new HashSet<TicketHistories>();
            TicketNotification = new HashSet<TicketNotification>();
        }



        
        public virtual ICollection<TicketAttachments> TicketAttachment { get; set; }
        public virtual ICollection<TicketHistories> TicketHistory { get; set; }
        public virtual ICollection<TicketComments> TicketComment { get; set; }
        public virtual ICollection<Projects> Project { get; set; }
        public virtual ICollection<TicketNotification> TicketNotification { get; set; }
        

        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public virtual DbSet<Tickets> Tickets { get; set; }
        public virtual DbSet<TicketAttachments> TicketAttachment { get; set; }
        public virtual DbSet<TicketHistories> TicketHistory { get; set; }
        public virtual DbSet<TicketComments> TicketComment { get; set; }
        public virtual DbSet<Projects> Project { get; set; }
        public virtual DbSet<TicketNotification> TicketNotification { get; set; }
        public virtual DbSet<TicketPriorities> TicketPriority { get; set; }
        public virtual DbSet<TicketStatuses> TicketStatus { get; set; }
       public virtual DbSet<TicketTypes> TicketType { get; set; }
        public IEnumerable ApplicationUsers { get; internal set; }
    }
}