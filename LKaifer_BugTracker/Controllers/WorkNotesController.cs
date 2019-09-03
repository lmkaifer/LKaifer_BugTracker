using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LKaifer_BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace LKaifer_BugTracker.Controllers
{
    public class WorkNotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkNotes
        public ActionResult Index()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId());
            var workNotes = currentUser.WorkNotes;
            return View(workNotes.ToList());
        }

        // GET: WorkNotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkNote workNote = db.WorkNotes.Find(id);
            if (workNote == null)
            {
                return HttpNotFound();
            }
            return View(workNote);
        }

        // GET: WorkNotes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: WorkNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body")] WorkNote workNote)
        {
            if (ModelState.IsValid)
            {
                workNote.UserId = User.Identity.GetUserId();
                workNote.Created = DateTime.Now;
                db.WorkNotes.Add(workNote);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", workNote.UserId);
            return View(workNote);
        }

        // GET: WorkNotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkNote workNote = db.WorkNotes.Find(id);
            if (workNote == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", workNote.UserId);
            return View(workNote);
        }

        // POST: WorkNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,UserId")] WorkNote workNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workNote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", workNote.UserId);
            return View(workNote);
        }

        // GET: WorkNotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkNote workNote = db.WorkNotes.Find(id);
            if (workNote == null)
            {
                return HttpNotFound();
            }
            return View(workNote);
        }

        // POST: WorkNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WorkNote workNote = db.WorkNotes.Find(id);
            db.WorkNotes.Remove(workNote);
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
