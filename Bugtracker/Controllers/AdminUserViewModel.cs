using System.Web.Mvc;
using Bugtracker.Models;

namespace Bugtracker.Controllers
{
    public class AdminViewModel
    {
        public MultiSelectList Role { get;  set; }
        public object SelectedRoles { get;  set; }
        public ApplicationUser User { get;  set; }
    }
}