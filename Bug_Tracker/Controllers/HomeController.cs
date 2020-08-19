using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Helpers;
using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{  
    public class HomeController : Controller
    {
        private RoleHelper roleHelper = new RoleHelper();
        private ProjectHelper projectHelper = new ProjectHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index()
        {
            AccountIndex ai = new AccountIndex();
            var user = db.Users.Find(User.Identity.GetUserId());
            ai.AllProjects = db.Projects.ToList();
            ai.User = user;
            ai.Role = roleHelper.ListUserRoles(User.Identity.GetUserId()).FirstOrDefault();
            if (ai.Role == "Admin")
            {
                ai.Projects = projectHelper.ListUserProjects(User.Identity.GetUserId());
                ai.Tickets = db.Tickets.ToList();
            }
            else if (ai.Role == "ProjectManager")
            {
                ai.Projects = projectHelper.ListUserProjects(User.Identity.GetUserId());
                ai.Tickets = projectHelper.ListUserProjects(User.Identity.GetUserId()).SelectMany(p => p.Tickets).ToList();
            }
            else if (ai.Role == "Developer")
            {
                ai.Projects = projectHelper.ListUserProjects(User.Identity.GetUserId());
                ai.Tickets = db.Tickets.Where(t => t.DeveloperId == user.Id).ToList();
            }
            else if (ai.Role == "Submitter")
            {
                ai.Projects = projectHelper.ListUserProjects(User.Identity.GetUserId());
                ai.Tickets = db.Tickets.Where(t => t.SubmitterId == user.Id).ToList();
            }

            return View(ai);
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        public PartialViewResult _LoginPartial()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return PartialView(user);
        }
    }
}