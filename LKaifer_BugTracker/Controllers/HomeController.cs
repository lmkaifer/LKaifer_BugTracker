using LKaifer_BugTracker.Helpers;
using LKaifer_BugTracker.Models;
using LKaifer_BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace LKaifer_BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult TicketStatusData()
        {

        var userId = User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myTickets = new List<Ticket>();

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
                    myTickets = db.Tickets.ToList();
                    break;
            }

            List<StatusTicketChartViewModel> resultList = myTickets.GroupBy(t => t.TicketStatus.Name).Select(g => new StatusTicketChartViewModel
            {
                TicketStatusName = g.Key,
                TicketCount = g.Count().ToString()
            }).ToList();

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TicketPriorityData()
        {
            var userId = User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myTickets = new List<Ticket>();

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
                    myTickets = db.Tickets.ToList();
                    break;
            }

            List<PriorityTicketChartViewModel> resultList = myTickets.GroupBy(t => t.TicketPriority.Name).Select(g => new PriorityTicketChartViewModel
            {
                TicketPriorityName = g.Key,
                TicketCount = g.Count().ToString()
            }).ToList();

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TicketTypeData()
        {
            var userId = User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myTickets = new List<Ticket>();

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
                    myTickets = db.Tickets.ToList();
                    break;
            }

            var totalTicketCount = myTickets.Count();
            List<TypeTicketChartViewModel> resultList = myTickets.GroupBy(t => t.TicketType.Name).Select(g => new TypeTicketChartViewModel
            {
                TicketTypeName = g.Key,
                TicketPercentage = ((double)g.Count() / totalTicketCount).ToString("0.00")
            }).ToList();

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SendEmailAjax(EmailViewModel model)
        {
            try
            {
                var fromUser = db.Users.Find(User.Identity.GetUserId());
                var toUser = db.Users.Find(model.ToUserId);
                var from = fromUser.Email;
                var to = toUser.Email;
                var email = new MailMessage(from, to)
                {
                    Subject = model.Subject,
                    Body = model.Body,
                    IsBodyHtml = true
                };
                var svc = new PersonalEmail();
                await svc.SendAsync(email);
                string sentConfirmationMessage = "Message has been successfully sent.";
                return Json(sentConfirmationMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}