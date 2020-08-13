using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
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

        // GET: Tickets
        public ActionResult Index()
        {
            var myTicketVM = new MyTicketViewModel();
            //myTicketVM.AllTickets = db.Tickets.ToList();
            var userId = User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            List<Ticket> model;
            switch (myRole)
            {
                case "Admin":
                    model = db.Tickets.ToList();
                    break;
                case "ProjectManager":
                case "Developer":
                    model = projectHelper.ListUserProjects(userId).SelectMany(p => p.Tickets).ToList();
                    break;

                case "Submitter":
                    model = db.Tickets.Where(t => t.SubmitterId == userId).ToList();
                    break;

                default:
                    return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
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

        // GET: Tickets/Create
        /*[Authorize(Roles = "Submitter")]*/
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
        public ActionResult Create([Bind(Include = "Id,Description,Title,DeveloperId")] Ticket ticket, int projectId , int ticketStatusId, int ticketPriorityId, int ticketTypeId)
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

        // GET: Tickets/Edit/5
        [Authorize(Roles = "ProjectManager")]
        public ActionResult Edit(int id)
        {

            Ticket ticket = db.Tickets.Find(id);
            var projectId = ticket.ProjectId;
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeveloperId = new SelectList(projectHelper.ListUsersOnProjectInRole(projectId, "Developer"), "Id", "FullName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
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
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId,SubmitterId,DeveloperId,Description,Title,Created,Updated,IsResolved,IsArchived")] Ticket ticket)
        {
            var projectId = ticket.ProjectId;
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
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
