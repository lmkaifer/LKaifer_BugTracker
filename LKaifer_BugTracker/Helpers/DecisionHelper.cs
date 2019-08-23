using LKaifer_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace LKaifer_BugTracker.Helpers
{
    public class TicketDecisionHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private static UserRolesHelper roleHelper = new UserRolesHelper();
        private static ProjectsHelper projectHelper = new ProjectsHelper();
       

        public static bool TicketIsEditableByUser(Ticket ticket)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch(myRole)
                {
                case "Developer":
                    return ticket.AssignedToUserId == userId;
                case "Submitter":
                    return ticket.OwnerUserId == userId;
                case "Manager":

                    var myProjects = projectHelper.ListUserProjects(userId);
                    foreach(var project in myProjects)
                    {
                        foreach(var projticket in project.Tickets)
                        {
                            if (projticket.Id == ticket.Id)
                                return true;
                        }
                    }
                    return false;

                case "Admin":
                    return true;
                default:
                    return false;

            }
        }
    }
}