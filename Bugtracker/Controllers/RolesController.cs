using Bugtracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Bugtracker.Controllers
{

    [Authorize]
    public class RoleController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper roleHelper = new UserRolesHelper();

        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRoles()
        {

            ViewBag.Users = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.Roles = new SelectList(db.Roles, "Name", "Name");

            return View();

        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(string Users, string Roles)
        {
            roleHelper.AddUserToRole(Users, Roles);

            ViewBag.Users = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.Roles = new SelectList(db.Roles, "Name", "Name");

            return View();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {



                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Roles = db.Roles.ToList();
            return View(Roles);

        }
        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {


                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Role = new IdentityRole();
            return View(Role);
        }

        /// <summary>
        /// Create a New Role
        /// </summary>  
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            db.Roles.Add(Role);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}