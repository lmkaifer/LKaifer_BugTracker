using LKaifer_BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            foreach(var item in statusticketlist)
            {
                StatusTicketChartViewModel viewmodel = new StatusTicketChartViewModel
                {
                    TicketStatusName = item.Name,
                    TicketCount = item.Tickets.Count.ToString()
                };
                resultList.Add(viewmodel);
            }
            
            return Json(resultList,JsonRequestBehavior.AllowGet);
        }
    }
}