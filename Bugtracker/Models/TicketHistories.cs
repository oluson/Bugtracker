using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class TicketHistories
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string Oldvalue { get; set; }
        public string NewValue { get; set; }
        public DateTime Updated { get; set; }
        public string UserId { get; set; }


        public virtual Tickets Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}