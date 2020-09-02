using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Helpers;
using Bug_Tracker.Models;
using Bug_Tracker.ViewModels;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();
        RoleHelper roleHelper = new RoleHelper();
        TicketHelper ticketHelper = new TicketHelper();
        private HistoriesHelper historyHelper = new HistoriesHelper();

        // GET: Tickets
        public ActionResult Index()
        {
            //var myTicketVM = new MyTicketViewModel();
            ////myTicketVM.AllTickets = db.Tickets.ToList();
            //var userId = User.Identity.GetUserId();
            //var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            //List<Ticket> model = new List<Ticket>();
            //switch (myRole)
            //{
            //    case "Admin":
            //        model = db.Tickets.ToList();
            //        break;
            //    case "ProjectManager":
            //    case "Developer":
            //        model = projectHelper.ListUserProjects(userId).SelectMany(p => p.Tickets).ToList();
            //        break;

            //    case "Submitter":
            //        model = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
            //        break;

            //    default:
            //        return RedirectToAction("Index", "Home");
            //}
            //return View(model);
            return View(db.Tickets.ToList());
        }

        public ActionResult GetProjectTickets()
        {
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);
            var ticketList = new List<Ticket>();
            ticketList = user.Projects.SelectMany(p => p.Tickets).ToList();
            return View("Index", ticketList);
        }

        public ActionResult GetMyTickets()
        {
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);
            var ticketList = new List<Ticket>();
            if (User.IsInRole("Developer"))
            {
                ticketList = db.Tickets.Where(t => t.DeveloperId == userid).ToList();
                return View("Index", ticketList);
            }
            if (User.IsInRole("Submitter"))
            {
                ticketList = db.Tickets.Where(t => t.SubmitterId == userid).ToList();
                return View("Index", ticketList);
            }
            else
            {
                return RedirectToAction("GetProjectTickets");
            }

        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            foreach (var notification in user.Notifications)
            {
                if (notification.TicketId == id)
                {
                    notification.IsRead = true;
                    db.SaveChanges();
                }
            }
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "None")]
        public ActionResult Create(int projectId)
        {
            var userId = User.Identity.GetUserId();
            if (!projectHelper.IsUserOnProject(userId, projectId))
            {
                return RedirectToAction("Details/" + projectId, "Projects");
            }

            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Title,DeveloperId")] Ticket ticket, int projectId, int ticketStatusId, int ticketPriorityId, int ticketTypeId)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                ticket.ProjectId = projectId;
                ticket.TicketPriorityId = ticketPriorityId;
                ticket.TicketStatusId = ticketStatusId;
                ticket.TicketTypeId = ticketTypeId;
                ticket.Created = DateTime.Now;
                ticket.SubmitterId = userId;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { ticket.Id });
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return RedirectToAction("Details/", "Projects", new { ticket.Id });
        }

        public ActionResult Rejection(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            var manager = projectHelper.ListUsersOnProjectInRole(ticket.Project.Id, "ProjectManager").FirstOrDefault();
            ViewBag.err = $"You do not have permission to edit this ticket. If you feel this is a mistake, please contact manager {manager.FullName} at {manager.Email}";
            return View();

        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager,Developer,Admin,Submitter")]
        public ActionResult Edit(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            if (!ticketHelper.CanEditTicket(id))
            {
                return RedirectToAction("Rejection", "Tickets", new { id });
            }


            var projectId = ticket.ProjectId;
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeveloperId = new SelectList(projectHelper.ListUsersOnProjectInRole(projectId, "Developer"), "Id", "FullName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(projectHelper.ListUsersOnProjectInRole(projectId, "Submitter"), "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId,SubmitterId,DeveloperId,Description,Title,Created,Updated,IsResolved,IsArchived")] Ticket ticket)
        {

            if (ModelState.IsValid)
            {
                if (ticket.DeveloperId != null)
                {
                    ticket.TicketStatusId = 2;
                }
                if(ticket.DeveloperId == null)
                {
                    ticket.TicketStatusId = 1;
                }
                if (ticket.IsResolved == true)
                {
                    ticket.TicketStatusId = 3;
                }
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                await ticketHelper.ManageTicketNotifications(oldTicket, newTicket, "no");
                historyHelper.ManageHistories(oldTicket, newTicket);
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName", ticket.DeveloperId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
