using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{
    public class Utilities
    { 
          private static ApplicationDbContext db = new ApplicationDbContext();
            public static string MakeReadable(string property, string value)
            {
                switch (property)
                {
                    case "TicketStatusId":
                        return db.TicketStatuses.Find(Convert.ToInt32(value)).Name;
                    case "TicketPriorityId":
                        return db.TicketPriorities.Find(Convert.ToInt32(value)).Name;
                    case "TicketTypeId":
                        return db.TicketTypes.Find(Convert.ToInt32(value)).Name;
                    case "AssignedToUserId":
                    case "OwnerUserId":
                        if (!string.IsNullOrEmpty(value))
                            return db.Users.Find(value).FirstName;
                        return "Unassigned";
                    default:
                        return value;
                }
            }
     }
 }

