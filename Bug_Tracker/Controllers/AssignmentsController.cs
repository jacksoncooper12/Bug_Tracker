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
        private ProjectHelper projectHelper = new ProjectHelper();
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
        public ActionResult ManageRoles(List<string> PM, List<string> DeveloperIds2, List<string> SubmitterIds2, List<string> NoRole, string roleName)
        {
            //Step 1: if anyone was selected, remove them from all of their roles
            var List = new List<string>();
            if (PM != null)
            {
                List.AddRange(PM);
            }
            if (DeveloperIds2 != null)
            {
                List.AddRange(DeveloperIds2);
            }
            if (SubmitterIds2 != null)
            {
                List.AddRange(SubmitterIds2);
            }
            if(NoRole != null)
            {
                List.AddRange(NoRole);
            }
            if (List == null)
            {
                return View();
            }
            //If people were selected, loop through and strip their roles
            foreach (var userId in List)
            {
                foreach (var role in roleHelper.ListUserRoles(userId).ToList())
                {
                    roleHelper.RemoveUserFromRole(userId, role);
                }
                //Step 2: if a role was chosen, add each person to that role
                if (!string.IsNullOrEmpty(roleName))
                {
                    roleHelper.AddUserToRole(userId, roleName);
                }
            }

            return RedirectToAction("Index", "Users");
        }


        public ActionResult ManageProjectUsers(int projectId)
        {
            var project = db.Projects.Find(projectId);
            ViewBag.UserIds = new MultiSelectList(projectHelper.ListUsersNotOnProject(projectId), "Id", "FullName");
            ViewBag.nonUserIds = new MultiSelectList(projectHelper.ListUsersOnProject(projectId), "Id", "FullName");
            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectUsers(List<string> userIds, int projectId)
        {
            if (userIds == null)
            {
                return RedirectToAction("ManageProjectUsers");
            }
            foreach (var userId in userIds)
            {
                if (userId != null)
                {
                    projectHelper.AddUserToProject(userId, projectId);
                }
            }
            ViewBag.UserIds = new MultiSelectList(projectHelper.ListUsersNotOnProject(projectId), "Id", "FullName");
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        public ActionResult RemoveProjectUsers(int projectId)
        {
            ViewBag.UserIds = new MultiSelectList(projectHelper.ListUsersOnProject(projectId), "Id", "FullName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProjectUsers(List<string> nonUserIds, int projectId)
        {
            if (nonUserIds == null)
            {
                return RedirectToAction("ManageProjectUsers");
            }
            foreach (var userId in nonUserIds)
            {
                if (userId != null)
                {
                    projectHelper.RemoveUserFromProject(userId, projectId);
                }
            }
            ViewBag.UserIds = new MultiSelectList(projectHelper.ListUsersOnProject(projectId), "Id", "FullName");
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}