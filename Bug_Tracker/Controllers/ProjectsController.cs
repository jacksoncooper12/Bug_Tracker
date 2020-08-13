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

namespace Bug_Tracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserHelper userHelper = new UserHelper();
        private RoleHelper roleHelper = new RoleHelper();
        private ProjectHelper projectHelper = new ProjectHelper();

        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);

            var tc = new TicketCreation();
            tc.project = db.Projects.Find(id);
            if (tc.project == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(tc);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Created,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.IsArchived = false;
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        public ActionResult ProjectWizard()
        {
            ViewBag.ProjectManagerId = new SelectList(roleHelper.UsersInRole("ProjectManager"), "Id", "FullName");
            ViewBag.DeveloperIds = new MultiSelectList(roleHelper.UsersInRole("Developer"), "Id", "FullName");
            ViewBag.SubmitterIds = new MultiSelectList(roleHelper.UsersInRole("Submitter"), "Id", "FullName");
            ViewBag.Errors = "";
            var model = new ProjectWizardVM();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectWizard(ProjectWizardVM model)
        {
            ViewBag.Errors = "";
            if(model.ProjectManagerId == null)
            {
                ViewBag.Errors += "<p>You must select a project Manager</p>";
            }
            if(model.DeveloperIds.Count == 0)
            {
                ViewBag.Errors += "<p>You must select at least one Developer</p>";
            }
            if (model.SubmitterIds.Count == 0)
            {
                ViewBag.Errors += "<p>You must select at least one Submitter</p>";
            }
            if(ViewBag.Errors.Length > 0)
            {
                ViewBag.ProjectManagerId = new SelectList(roleHelper.UsersInRole("ProjectManager"), "Id", "FullName");
                ViewBag.DeveloperIds = new MultiSelectList(roleHelper.UsersInRole("Developer"), "Id", "FullName");
                ViewBag.SubmitterIds = new MultiSelectList(roleHelper.UsersInRole("Submitter"), "Id", "FullName");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = model.Name;
                project.IsArchived = false;
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();

                projectHelper.AddUserToProject(model.ProjectManagerId, project.Id);
                foreach(var userId in model.DeveloperIds)
                {
                    projectHelper.AddUserToProject(userId, project.Id);

                }
                foreach (var userId in model.SubmitterIds)
                {
                    projectHelper.AddUserToProject(userId, project.Id);

                }

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ProjectManagerId = new SelectList(roleHelper.UsersInRole("ProjectManager"), "Id", "FullName");
                ViewBag.DeveloperIds = new MultiSelectList(roleHelper.UsersInRole("Developer"), "Id", "FullName");
                ViewBag.SubmitterIds = new MultiSelectList(roleHelper.UsersInRole("Submitter"), "Id", "FullName");
                return View(model);
            }

        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
