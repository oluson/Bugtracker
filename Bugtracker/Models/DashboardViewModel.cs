using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugtracker.Models
{
    public class DashboardViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public List<Projects> Projects { get; set; }
        public List<Tickets> Tickets { get; set; }
    }
}