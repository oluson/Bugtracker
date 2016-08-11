using Bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugtracker.Controllers
{
    public class HomeController : Controller
    {
       // private UserRolesHelper Urh = new UserRolesHelper(new ApplicationDbContext);
        public ActionResult Index()
        {
        
            return View();
        }

        public ActionResult About(string uid, string role)
        {
            //UserRolesHelper helper = new UserRolesHelper();
            //helper.AddUserToRole(uid, role);


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}