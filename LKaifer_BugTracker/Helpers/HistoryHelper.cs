using LKaifer_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace LKaifer_BugTracker.Helpers
{
    public class HistoryHelper : CommonHelper
    {
        public static void AnyChanges(Ticket oldTicket, Ticket newTicket)
        {
                var trackedProperties = WebConfigurationManager.AppSettings["TrackedHistoryProperties"].Split(',').ToList();
            foreach (var properties in newTicket.GetType().GetProperties())
            {
                if (!trackedProperties.Contains(properties.Name))
                {
                    continue;
                }
                var newValue = (properties.GetValue(newTicket, null) ?? "").ToString();
                var oldValue = (properties.GetValue(oldTicket, null) ?? "").ToString();
                //var oldValue = oldTicket.GetType().GetProperty(properties).GetValue(oldTicket, null).ToString();

                //var newValue = newTicket.GetType().GetProperty(properties).GetValue(newTicket, null).ToString();
               if (oldValue != newValue)
                {
                    var newHistory = new TicketHistory
                    {
                        UserId = HttpContext.Current.User.Identity.GetUserId(),
                        Updated = (DateTime)newTicket.Updated,
                        PropertyName = properties.Name,
                        OldValue = Utilities.MakeReadable(properties.Name, oldValue),
                        NewValue = Utilities.MakeReadable(properties.Name, newValue),
                        TicketId = newTicket.Id
                    };

                    db.TicketHistories.Add(newHistory);
                    db.SaveChanges();
                }
            }

        }

    }
}