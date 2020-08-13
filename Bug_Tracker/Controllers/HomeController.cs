using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{  
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index()
        {
            return View();
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