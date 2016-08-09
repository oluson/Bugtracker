using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class Projects
    {
        public Projects()
        {
            Tickets = new HashSet<Tickets>();
            ProjectUsers = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Tickets> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> ProjectUsers { get; set; }



    }
}