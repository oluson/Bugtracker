using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<Tickets> Tickets { get; set; }
        public IEnumerable<TicketAttachments> Attachments { get; set; }
        public IEnumerable<TicketComments> Comments { get; set; }
        public int ProjectsAmt { get; set; }
        public string Projects { get; set; }
    }
}