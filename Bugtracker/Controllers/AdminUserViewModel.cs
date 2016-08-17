using System.Web.Mvc;
using Bugtracker.Models;

namespace Bugtracker.Controllers
{
    public class AdminUserViewModel
    {
        public MultiSelectList Role { get; internal set; }
        public object SelectedRoles { get; internal set; }
        public ApplicationUser User { get; internal set; }
    }
}