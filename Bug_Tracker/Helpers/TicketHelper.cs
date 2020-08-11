using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Bug_Tracker.Helpers
{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        //public List<Ticket> GetMyTickets()
        //{
        //    var userId = HttpContext.Current.User.Identity.GetUserId();
        //    var tickets = new List<Ticket>();
        //    switch (myRole)
        //    {
        //        case "Admin":
        //            model = db.Tickets.ToList();
        //            break;
        //        case "ProjectManager":
        //        case "Developer":
        //            model = projectHelper.ListUserProjects(userId).SelectMany(p => p.Tickets).ToList();
        //            break;

        //        case "Submitter":
        //            model = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
        //            break;

        //        default:
        //            return RedirectToAction("Index", "Home");
        //    }

        //}
    }
}