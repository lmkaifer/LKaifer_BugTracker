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

        public ActionResult Index()
        {
            ViewBag.AllProjectList = db.Projects.ToList();


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
            var statusticketlist = db.TicketStatuses.ToList();
            List<StatusTicketChartViewModel> resultList = new List<StatusTicketChartViewModel>();
            foreach (var item in statusticketlist)
            {
                StatusTicketChartViewModel viewmodel = new StatusTicketChartViewModel
                {
                    TicketStatusName = item.Name,
                    TicketCount = item.Tickets.Count.ToString()
                };
                resultList.Add(viewmodel);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TicketPriorityData()
        {
            var prioritiesticketlist = db.TicketPriorities.ToList();
            List<PriorityTicketChartViewModel> resultList = new List<PriorityTicketChartViewModel>();
            foreach (var item in prioritiesticketlist)
            {
                PriorityTicketChartViewModel viewmodel = new PriorityTicketChartViewModel
                {
                    TicketPriorityName = item.Name,
                    TicketCount = item.Tickets.Count.ToString()
                };
                resultList.Add(viewmodel);
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TicketTypeData()
        {
            var typesticketlist = db.TicketTypes.ToList();
            var totalTicketCount = db.Tickets.Count();
            List<TypeTicketChartViewModel> resultList = new List<TypeTicketChartViewModel>();
            foreach (var item in typesticketlist)
            {
                TypeTicketChartViewModel viewmodel = new TypeTicketChartViewModel();
                var percentageResult = (double)item.Tickets.Count / totalTicketCount;
                viewmodel.TicketTypeName = item.Name;
                viewmodel.TicketPercentage = percentageResult.ToString("0.00");
                resultList.Add(viewmodel);
            }

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