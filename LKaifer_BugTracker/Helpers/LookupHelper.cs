using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{
    public class LookupHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetNamefromId (string propertyName, string id)
        {
            var name = "";

            if (propertyName == "TicketPriorityId")
            {
                name = db.TicketPriorities.Find(Convert.ToInt32(id)).Name;

            }
            else if (propertyName == "TicketStatusId")
            {
                name = db.TicketStatuses.Find(Convert.ToInt32(id)).Name;
            }
            else if (propertyName == "TicketTypeId")
            {
                name = db.TicketTypes.Find(Convert.ToInt32(id)).Name;

            }
            else if (propertyName == "AssignedToUser")
            {
                name = db.Users.Find(id).FullName;
            }
            return name;
        }
    }
}