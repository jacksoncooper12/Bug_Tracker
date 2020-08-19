using Bug_Tracker.Helpers;
using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bug_Tracker.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        private UserHelper userHelper = new UserHelper();
        // GET: Users
        public ActionResult Index(string role)
        {
            var users = db.Users.ToList();
            if (role != null)
            {
                var sUsers = roleHelper.UsersInRole(role);
                if (role == "ProjectManager")
                {
                    ViewBag.Role = "Project Manager";
                }
                else
                {
                    ViewBag.Role = role;
                }

                return View(sUsers);
            }
            ViewBag.Role = "All User";
            return View(db.Users.ToList());
        }
        public ActionResult ManageUserRole(string id)
        {
            var userRole = roleHelper.ListUserRoles(id).FirstOrDefault();
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name", userRole);
            return View(db.Users.Find(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserRole(string id, string roleName)
        {
            //spin through all roles for user and remove them
            foreach(var role in roleHelper.ListUserRoles(id))
            {
                roleHelper.RemoveUserFromRole(id, role);
            }
            //if a role is chosen, add user to that role
            if (!string.IsNullOrEmpty(roleName))
            {
                roleHelper.AddUserToRole(id, roleName);
            }
            //now that the new role is assigned, redirect to the page from which they came
            return RedirectToAction("ManageUserRole", new { id });
        }
    }
}