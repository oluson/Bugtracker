using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
         [Required]
        public string Title { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public bool Archived { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Tickets> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> ProjectUsers { get; set; }



    }
}