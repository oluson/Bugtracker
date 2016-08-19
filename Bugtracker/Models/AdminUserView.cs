using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugtracker.Models
{
    public class AdminUserView
    {
        public MultiSelectList Role { get; set; }
        public object SelectedRoles { get; set; }
        public ApplicationUser User { get; set; }
    }
}