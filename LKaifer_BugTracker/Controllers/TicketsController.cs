using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LKaifer_BugTracker.Helpers;
using LKaifer_BugTracker.Models;
using LKaifer_BugTracker.Utilities;
using Microsoft.AspNet.Identity;

namespace LKaifer_BugTracker.Controllers
{
    [RequireHttps]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();

        [Authorize(Roles = "Admin, Manager")]

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }
        [Authorize(Roles = "Admin, Manager, Developer, Submitter")]
        public ActionResult MyIndex(string unassignedTicketsOnly)
        {
            //My index wants to fill some view with MY Tickets only. If I am a submitter, my Tickets are the Tickers

            //First get the Id of the logged in User
            var userId = User.Identity.GetUserId();

            //Then get the Role they occupy
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myTickets = new List<Ticket>();
            //Then based on the Role Name we will push different data into the view
            if(!string.IsNullOrEmpty(unassignedTicketsOnly))
            {
                myTickets = db.Tickets.Where(t => t.AssignedToUserId == null).ToList();
            }
            else
            {
                switch (myRole)
                {

                    case "Developer":
                        myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                        break;
                    case "Submitter":
                        myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                        break;
                    case "Manager":
                        //myTickets are going to be all the Tickets on all the Projects I am on.
                        myTickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
                        break;
                    case "Admin":
                        myTickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
                        break;
                }
            }
            return View("Index", myTickets);


            //where the OwnerUserId equals my Id. If I am the Developer, my Tickets are the Tickets wehre the AssignedToUserId is my Id

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
            var myProjects = projectHelper.ListUserProjects(User.Identity.GetUserId());
            ProjectsHelper projectshelper = new ProjectsHelper();
            List<ApplicationUser> projectusers = new List<ApplicationUser>();
            List<string> projectDevIds = new List<string>();
            foreach (Project project in myProjects)
            {
                projectDevIds = projectshelper.UsersInRoleOnProject(project.Id, "Developer");
            }
            foreach (string devId in projectDevIds)
            {
                ApplicationUser devUser = db.Users.Find(devId);
                projectusers.Add(devUser);
            }
            ViewBag.AssignedToUserId = new SelectList(projectusers, "Id", "DisplayName", ticket.AssignedToUserId);

            return View(ticket);
        }

        //POST:  Tickets/Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string AssignedToUserId, int ticketid)
        {
            Ticket ticket = db.Tickets.Find(ticketid);
            ticket.AssignedToUserId = AssignedToUserId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketid });
        }



        [Authorize(Roles = "Submitter")]
        // GET: Tickets/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                var myProjects = projectHelper.ListUserProjects(User.Identity.GetUserId());
                ViewBag.ProjectId = new SelectList(myProjects, "Id", "Name");

            }
            else
            {
                ViewBag.ProjectId = id;
                ViewBag.ProjectExist = db.Projects.Find(id);
            }
            //Obtaining only projects to which the person is assigned, not all projects
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,TicketTypeId,TicketPriorityId,Title,Description")] Ticket ticket, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                List<Tuple<string, string, string>> fileUploadResult = FileUploadHelper.UploadMultipleFiles(files);
                foreach (Tuple<string, string, string> upload in fileUploadResult)
                {
                    TicketAttachment ticketAttachment = new TicketAttachment();
                    ticketAttachment.Title = upload.Item1;
                    ticketAttachment.AttachmentUrl = upload.Item2;
                    ticketAttachment.Description = upload.Item3;
                    ticketAttachment.Created = DateTime.Now;
                    ticketAttachment.UserId = User.Identity.GetUserId();
                    db.TicketAttachments.Add(ticketAttachment);
                    ticket.TicketAttachments.Add(ticketAttachment);
                    HistoryHelper.AnyAttachment(ticket, ticketAttachment, true);
                }
                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(t => t.Name == "New/UnAssigned").Id;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                HistoryHelper.AnyCreated(ticket);
                return RedirectToAction("MyIndex");
            }

            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Manager, Submitter, Developer")]
        public ActionResult Edit(int? id)
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


            if (TicketDecisionHelper.TicketIsEditableByUser(ticket))
            {
                //UserRolesHelper helper = new UserRolesHelper();
                //var users = helper.UsersInRole("Developer").ToList();
                //ViewBag.AssignedToUserId = new SelectList(users, "Id", "FullName", ticket.AssignedToUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
            }
            else
            {
                return RedirectToAction("AccessViolation", "Admin");
            }
        }




        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,OwnerUserId,AssignedToUserId,Title,Description,Created,Updated")] Ticket ticket, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {


                List<Tuple<string, string, string>> fileUploadResult = FileUploadHelper.UploadMultipleFiles(files);
                foreach (Tuple<string, string, string> upload in fileUploadResult)
                {
                    TicketAttachment ticketAttachment = new TicketAttachment();
                    ticketAttachment.Title = upload.Item1;
                    ticketAttachment.AttachmentUrl = upload.Item2;
                    ticketAttachment.Description = upload.Item3;
                    ticketAttachment.Created = DateTime.Now;
                    ticketAttachment.UserId = User.Identity.GetUserId();
                    ticketAttachment.TicketId = ticket.Id;
                    db.TicketAttachments.Add(ticketAttachment);
                    ticket.TicketAttachments.Add(ticketAttachment);
                    HistoryHelper.AnyAttachment(ticket, ticketAttachment, true);
                }

                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                ticket.Updated = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                var newTicket = db.Tickets.Where(n => n.Id == ticket.Id).Include(n => n.TicketPriority).Include(n => n.TicketStatus).Include(n => n.TicketType).FirstOrDefault();
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                await NotificationHelper.ManageNotifications(oldTicket, newTicket, callbackUrl, false);
                HistoryHelper.AnyChanges(oldTicket, ticket);
                return RedirectToAction("MyIndex");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
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

        [Authorize(Roles = "Admin, Manager")]

        public ActionResult AssignTicket(int? id)
        {
            UserRolesHelper helper = new UserRolesHelper();
            var ticket = db.Tickets.Find(id);
            var users = helper.UsersInRole("Developer").ToList();
            var currentTicketDevelopers = users.Where(u => u.Projects.Any(p => p.Id.Equals(ticket.ProjectId)));
            ViewBag.AssignedToUserId = new SelectList(currentTicketDevelopers, "Id", "FullName", ticket.AssignedToUserId);

            return View(ticket);

        }
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignTicket(Ticket model)
        {
            var ticket = db.Tickets.Find(model.Id);
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
            ticket.AssignedToUserId = model.AssignedToUserId;
            ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "Assigned")?.Id ?? 1;
            db.SaveChanges();

            if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
            {
                string oldTicketAssignedUserFullName = oldTicket.AssignedToUser != null ? oldTicket.AssignedToUser.FullName : "None";
                await NotificationHelper.ManageNotifications(oldTicket, ticket, callbackUrl, true);
                TicketHistory history = new TicketHistory
                {
                    PropertyName = "Assigned User",
                    OldValue = oldTicketAssignedUserFullName,
                    NewValue = ticket.AssignedToUser.FullName,
                    TicketId = ticket.Id,
                    Updated = DateTime.Now,
                    UserId = User.Identity.GetUserId()
                };

                db.TicketHistories.Add(history);
                db.SaveChanges();
            }


            try
            {

                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(model.AssignedToUserId);
                msg.Body = "You have been assigned a new ticket."
                    + Environment.NewLine
                    + "Please click the following line to view the details"
                    + "<a href =\"" + callbackUrl + "\">NEW TICKET</a>";

                msg.Destination = user.Email;
                msg.Subject = "New Ticket Assignment";

                await ems.SendAsync(msg);

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
            return RedirectToAction("MyIndex");
        }
        public ActionResult MarkInProgress(int? id)
        {

            var ticket = db.Tickets.Find(id);
            ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "In Progress")?.Id ?? 1;
            db.SaveChanges();
            return Redirect(Request.ServerVariables["http_referer"]);

        }

        public ActionResult MarkAsCompleted(int? id)
        {
            var ticket = db.Tickets.Find(id);
            ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "Completed")?.Id ?? 1;
            db.SaveChanges();
            return Redirect(Request.ServerVariables["http_referer"]);
        }

        public ActionResult MarkAsArchived(int? id)
        {
            var ticket = db.Tickets.Find(id);
            ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(s => s.Name == "Archived")?.Id ?? 1;
            db.SaveChanges();
            return Redirect(Request.ServerVariables["http_referer"]);

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
