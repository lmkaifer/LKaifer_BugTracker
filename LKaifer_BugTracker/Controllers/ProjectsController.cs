using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LKaifer_BugTracker.Helpers;
using LKaifer_BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace LKaifer_BugTracker.Controllers
{
    [Authorize(Roles = "Admin, Manager, Submitter, Developer")]
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();
        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }
        [Authorize(Roles = "Admin, Manager, Submitter, Developer")]
        public ActionResult MyIndex()
        {
            
            var userId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(userId);
            var myProjects = currentUser.Projects.ToList();
            
          return View(myProjects);

            

        }
        [Authorize(Roles = "Admin, Manager, Submitter, Developer")]
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
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

            
           

            var allProjectManagers = roleHelper.UsersInRole("Manager");
            var currentProjectManagers = projectHelper.UsersInRoleOnProject(project.Id, "Manager");
            ViewBag.ProjectManagers = new MultiSelectList(allProjectManagers, "Id", "FullNameWithEmail", currentProjectManagers);

            var allSubmitters = roleHelper.UsersInRole("Submitter");
            var currentProjectSubmitters = projectHelper.UsersInRoleOnProject(project.Id, "Submitter");
            ViewBag.Submitters = new MultiSelectList(allSubmitters, "Id", "FullNameWithEmail", currentProjectSubmitters);

            var allDevelopers = roleHelper.UsersInRole("Developer");
            var currentProjectDevelopers = projectHelper.UsersInRoleOnProject(project.Id, "Developer");
            ViewBag.Developers = new MultiSelectList(allDevelopers,"Id", "FullNameWithEmail", currentProjectDevelopers);

            var allAdministrators = roleHelper.UsersInRole("Admin");
            var currentProjectAdmnistrators = projectHelper.UsersInRoleOnProject(project.Id, "Admin");
            ViewBag.Administrators = new MultiSelectList(allAdministrators, "Id", "FullNameWithEmail", currentProjectAdmnistrators);


            return View(project);
        }
        [Authorize(Roles = "Admin, Manager")]
        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                project.Created = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }
        [Authorize(Roles = "Admin")]
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        [Authorize(Roles = "Admin")]
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
