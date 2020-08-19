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
        private HistoriesHelper historyHelper = new HistoriesHelper();
        private ProjectHelper projectHelper = new ProjectHelper();
        public List<Ticket> GetMyTickets()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var tickets = new List<Ticket>();
            switch (myRole)
            {
                case "Admin":
                    tickets = db.Tickets.ToList();
                    return tickets;
                case "ProjectManager":
                    foreach (var item in projectHelper.ListUserProjects(userId))
                    {
                        foreach (var ticket in item.Tickets)
                        {
                            tickets.Append(ticket);
                        }
                    };
                    return tickets;
                case "Developer":
                    tickets = projectHelper.ListUserProjects(userId).SelectMany(p => p.Tickets).ToList();
                    return tickets;

                case "Submitter":
                    tickets = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
                    return tickets;

                default:
                    return null;
            }

        }
        public void ManageTicketNotifications(Ticket oldTicket, Ticket newTicket)
        {
            if (oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId != null && oldTicket.DeveloperId != null)
            {
                var notification = new TicketNotification()
                {
                    UserId = newTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    User = db.Users.Find(newTicket.DeveloperId),
                    Message = $"You have been assigned to a Ticket"
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
                var notification2 = new TicketNotification()
                {
                    
                    UserId = oldTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = oldTicket.Id,
                    User = db.Users.Find(oldTicket.DeveloperId),
                    Message = $"You have been removed from a Ticket"
                };
                db.TicketNotifications.Add(notification2);
                db.SaveChanges();
            }
            else if (oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId != null)
            {
                var notification = new TicketNotification()
                {
                    UserId = newTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    User = db.Users.Find(newTicket.DeveloperId),
                    Message = $"You have been assigned to a Ticket"
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }
            else if (oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId == null)
            {
                var notification = new TicketNotification()
                {
                    UserId = oldTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = oldTicket.Id,
                    User = db.Users.Find(oldTicket.DeveloperId),
                    Message = $"You have been removed from a Ticket"
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }


        }
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
        public void EditedTicket(Ticket oldTicket, Ticket newTicket)
        {
            historyHelper.ManageHistories(oldTicket, newTicket);
            ManageTicketNotifications(oldTicket, newTicket);
        }
    }
}