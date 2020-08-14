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
        public bool CanEditTicket(int ticketId)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                    return true;
                case "ProjectManager":
                    var user = db.Users.Find(userId);
                    var projects = user.Projects;
                    var tickets = projects.SelectMany(p => p.Tickets);
                    var bool1 = tickets.Any(t => t.Id == ticketId);
                    return user.Projects.SelectMany(p => p.Tickets).Any(t => t.Id == ticketId);
                case "Developer":
                case "Submitter":
                    var ticket = db.Tickets.Find(ticketId);
                    if (ticket.DeveloperId == userId || ticket.SubmitterId == userId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
        public bool CanMakeComment(int ticketId)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                    return true;
                case "ProjectManager":
                    var user = db.Users.Find(userId);
                    var projects = user.Projects;
                    var tickets = projects.SelectMany(p => p.Tickets);
                    var bool1 = tickets.Any(t => t.Id == ticketId);
                    return user.Projects.SelectMany(p => p.Tickets).Any(t => t.Id == ticketId);
                case "Developer":
                case "Submitter":
                    var ticket = db.Tickets.Find(ticketId);
                    if (ticket.DeveloperId == userId || ticket.SubmitterId == userId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }

        }
    }
}