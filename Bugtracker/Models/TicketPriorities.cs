using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class TicketPriorities
    {
        public int Id { get; set; }

        public int TicketId { get; set; }


         public virtual Tickets Ticket { get; set; }
    }
}