using Bug_Tracker.Helpers;
using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Bug_Tracker.Controllers
{
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        // GET: Assignments
        public ActionResult ManageRoles()
        {
            //use viewbag to hold a multiselect list of users in system
            //multiselectlist arguments: (the data, "Id", "display (email recommended)")
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email");
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");
            return View(db.Users.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(List<string>userIds, string roleName)
        {
            //Step 1: if anyone was selected, remove them from all of their roles
            if(userIds == null)
            {
                return RedirectToAction("RolesIndex");
            }
            //If people were selected, loop through and strip their roles
            foreach(var userId in userIds)
            {
                foreach(var role in roleHelper.ListUserRoles(userId).ToList())
                {
                    roleHelper.RemoveUserFromRole(userId, role);
                }
                if (!string.IsNullOrEmpty(roleName))
                {
                    roleHelper.AddUserToRole(userId, roleName);
                }
            }
            //Step 2: if a role was chosen, add each person to that role
            return RedirectToAction("ManageRoles");
        }
    }
}