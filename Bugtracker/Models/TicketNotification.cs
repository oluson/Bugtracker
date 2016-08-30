﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId{ get; set; }
        public string UserId { get; set; }
        public bool message { get; set; }
        public bool IsRead { get; set; }


        public virtual Tickets Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

       
    }
}