using Bugtracker.Models;
using System.Web.Mvc;


namespace Bugtracker.Models
{ 
public class AdminUserViewModel
{
    public ApplicationUser User { get; set; }
    public MultiSelectList Roles { get; set; }
    public string[] SelectedRoles { get; set; }
}
}