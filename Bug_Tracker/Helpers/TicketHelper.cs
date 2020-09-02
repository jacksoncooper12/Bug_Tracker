using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Bug_Tracker.Helpers
{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        private HistoriesHelper historyHelper = new HistoriesHelper();
        private ProjectHelper projectHelper = new ProjectHelper();
        EmailService svc = new EmailService();
        readonly string from = "BugTracker<jacksoncooper12@gmail.com>";
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
                            tickets.Add(ticket);
                        }
                    };
                    return tickets;
                case "Developer":
                    tickets = db.Tickets.Where(t => t.DeveloperId == userId).ToList();
                    return tickets;

                case "Submitter":
                    tickets = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
                    return tickets;
                case "Unassigned":
                    tickets = db.Tickets.ToList();
                    return tickets;

                default:
                    return null;
            }

        }
        public List<TicketComment> GetMyComments()
        {
            var tickets = GetMyTickets();
            var comments = new List<TicketComment>();
            foreach (var ticket in tickets)
            {
                foreach (var comment in ticket.TicketComments)
                {
                    comments.Add(comment);
                }
            }
            return comments;
        }
        public async Task ManageTicketNotifications(Ticket oldTicket, Ticket newTicket, string yes)
        {
            if (oldTicket.DeveloperId != newTicket.DeveloperId && newTicket.DeveloperId != null && oldTicket.DeveloperId != null)
            {
                var notification = new TicketNotification()
                {
                    UserId = newTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    User = db.Users.Find(newTicket.DeveloperId),
                    Message = $"You have been assigned to a Ticket."
                };
                db.TicketNotifications.Add(notification);
                try
                {
                    var email = new MailMessage(from, notification.User.Email)
                    {
                        Subject = "Ticket Assignment",
                        Body = notification.Message + " <a href='https://jcbugtracker.azurewebsites.net/'>Log in for more information.</a>",
                        IsBodyHtml = true
                    };
                    await svc.SendAsync(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
                db.SaveChanges();
                var notification2 = new TicketNotification()
                {

                    UserId = oldTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = oldTicket.Id,
                    User = db.Users.Find(oldTicket.DeveloperId),
                    Message = $"You have been removed from a Ticket."
                };
                db.TicketNotifications.Add(notification2);
                try
                {
                    var email = new MailMessage(from, notification2.User.Email)
                    {
                        Subject = "Ticket Unassignment",
                        Body = notification2.Message + " <a href='https://jcbugtracker.azurewebsites.net/'>Log in for more information.</a>",
                        IsBodyHtml = true
                    };
                    await svc.SendAsync(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
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
                    Message = $"You have been assigned to a Ticket."
                };
                db.TicketNotifications.Add(notification);
                try
                {
                    var email = new MailMessage(from, notification.User.Email)
                    {
                        Subject = "Ticket Assignment",
                        Body = notification.Message + " <a href='https://jcbugtracker.azurewebsites.net/'>Log in for more information.</a>",
                        IsBodyHtml = true
                    };
                    await svc.SendAsync(email);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
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
                    Message = $"You have been removed from a Ticket."
                };
                db.TicketNotifications.Add(notification);
                try
                {
                    var email = new MailMessage(from, notification.User.Email)
                    {
                        Subject = "Ticket Unassignment",
                        Body = notification.Message + " <a href='https://jcbugtracker.azurewebsites.net/'>Log in for more information.</a>",
                        IsBodyHtml = true
                    };
                    await svc.SendAsync(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
                db.SaveChanges();
            }
            if (oldTicket.TicketAttachments.Count() != newTicket.TicketAttachments.Count() && newTicket.DeveloperId != null)
            {
                var notification = new TicketNotification()
                {
                    UserId = newTicket.DeveloperId,
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    User = db.Users.Find(newTicket.DeveloperId),
                    Message = $"A new attachment has been added to one of your Tickets."
                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }
            if (oldTicket.IsResolved == false && newTicket.IsResolved == true)
            {
                var notification = new TicketNotification();
                notification.UserId = projectHelper.ListUsersOnProjectInRole(newTicket.Project.Id, "ProjectManager").FirstOrDefault().Id;
                notification.Created = DateTime.Now;
                notification.TicketId = newTicket.Id;
                notification.User = db.Users.Find(projectHelper.ListUsersOnProjectInRole(newTicket.Project.Id, "ProjectManager").FirstOrDefault().Id);
                notification.Message = $"A Ticket has been checked off as resolved on one of your projects. Archive the ticket to approve this change.";
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }
            if (yes == "yes")
            {
                if (newTicket.TicketComments.FirstOrDefault().Comment.Contains("[HELP]"))
                {
                    var newnotification = new TicketNotification();
                    newnotification.UserId = projectHelper.ListUsersOnProjectInRole(newTicket.Project.Id, "ProjectManager").FirstOrDefault().Id;
                    newnotification.Created = DateTime.Now;
                    newnotification.TicketId = newTicket.Id;
                    newnotification.User = db.Users.Find(projectHelper.ListUsersOnProjectInRole(newTicket.Project.Id, "ProjectManager").FirstOrDefault().Id);
                    newnotification.Message = $"A developer has requested help on one of your Projects' Tickets.";
                    db.TicketNotifications.Add(newnotification);
                    db.SaveChanges();
                }
                var notification = new TicketNotification();
                notification.UserId = newTicket.DeveloperId;
                notification.Created = DateTime.Now;
                notification.TicketId = newTicket.Id;
                notification.User = db.Users.Find(newTicket.DeveloperId);
                notification.Message = $"A comment has been added to one of your Tickets.";
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
    }
}