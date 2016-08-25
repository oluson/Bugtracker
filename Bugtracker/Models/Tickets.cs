using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bugtracker.Models
{
    public class Tickets
    {

        public Tickets()
        {
            TicketAttachment = new HashSet<TicketAttachments>();
            TicketComment = new HashSet<TicketComments>();
            TicketHistory = new HashSet<TicketHistories>();
            TicketNotification = new HashSet<TicketNotification>();


        }


        public int Id { get; set; }

        public string Title { get; set; }
        [AllowHtml]
        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public int ProjectId { get; set; }

        public int TicketTypeId { get; set; }

        public int TicketPriorityId { get; set; }

        public int TicketStatusId { get; set; }

        public string OwnerUserId { get; set; }

        public string AssignedToUserId { get; set; }







        public virtual ICollection<TicketAttachments> TicketAttachment { get; set; }
        public virtual ICollection<TicketHistories> TicketHistory { get; set; }
        public virtual ICollection<TicketComments> TicketComment { get; set; }
        public virtual ICollection<TicketNotification> TicketNotification { get; set; }

        public virtual TicketPriorities TicketPriority { get; set; }
        public virtual Projects Project { get; set; }
        public virtual TicketStatuses TicketStatus { get; set; }
        public virtual TicketTypes TicketType { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

       
    }
}