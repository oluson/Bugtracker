
using System.Collections.Generic;

namespace Bugtracker.Models
{
    public class TicketStatuses
    {
        public TicketStatuses()
        {
           Ticket = new HashSet<Tickets>();
           
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Tickets> Ticket { get; set; }


    }
}