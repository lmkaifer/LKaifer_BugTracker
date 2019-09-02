using LKaifer_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Routing;

namespace LKaifer_BugTracker.Helpers
{
    public class NotificationHelper : CommonHelper
    {
        private static string _callbackUrl = "";
        public static async Task<bool> ManageNotifications(Ticket oldTicket, Ticket newTicket, string callbackUrl, bool isUserAssign)
        {
            bool success = false;
            _callbackUrl = callbackUrl;
            try
            {
                if (isUserAssign)
                    success = await CreateAssignmentNotification(oldTicket, newTicket);
                else
                    success = await CreateChangeNotification(oldTicket, newTicket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }

            return success;
        }

        private static async Task<bool> CreateChangeNotification(Ticket oldTicket, Ticket newTicket)
        {
            bool success = false;
            try
            {
                var messageBody = new StringBuilder();
                foreach (var property in WebConfigurationManager.AppSettings["TrackedTicketProperties"].Split(','))
                {
                    string oldValue = String.Empty;
                    string newValue = String.Empty;
                    string userFriendlyProp = String.Empty;

                    switch (property)
                    {
                        case "TicketPriorityId":
                            userFriendlyProp = "Ticket Priority";
                            oldValue = oldTicket.TicketPriority.Name;
                            newValue = newTicket.TicketPriority.Name;
                            break;
                        case "TicketStatusId":
                            userFriendlyProp = "Ticket Status";
                            oldValue = oldTicket.TicketStatus.Name;
                            newValue = newTicket.TicketStatus.Name;
                            break;
                        case "TicketTypeId":
                            userFriendlyProp = "Ticket Type";
                            oldValue = oldTicket.TicketType.Name;
                            newValue = newTicket.TicketType.Name;
                            break;
                        default:
                            userFriendlyProp = property;
                            oldValue = oldTicket.GetType().GetProperty(property).GetValue(oldTicket, null).ToString();
                            newValue = newTicket.GetType().GetProperty(property).GetValue(newTicket, null).ToString();
                            break;
                    }

                    if (oldValue != newValue)
                    {
                        messageBody.AppendLine(new String('-', 45));
                        messageBody.AppendLine($"A change was made to Property:{userFriendlyProp}. ");
                        messageBody.AppendLine($"The old value is: {oldValue}. ");
                        messageBody.AppendLine($"The new value is: {newValue}. ");
                    }
                }

                if (!string.IsNullOrEmpty(messageBody.ToString()))
                {
                    var msgHeader = $"The following changes were made to {oldTicket.Title} on {newTicket.Updated}";
                    var senderId = HttpContext.Current.User.Identity.GetUserId();
                    var notification = new TicketNotification
                    {
                        Created = DateTime.Now,
                        Subject = msgHeader,
                        Read = false,
                        RecipientId = oldTicket.AssignedToUserId,
                        SenderId = senderId,
                        NotificationBody = messageBody.ToString(),
                        TicketId = newTicket.Id
                    };
                    db.TicketNotifications.Add(notification);
                    db.SaveChanges();
                    success = await EmailNotification(notification);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }
            return success;
        }

        private static async Task<bool> CreateAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            //There are 4 cases:
            //1. No change. . 
            //2. An assignment - This means that the old AssignedToUserId was null and the current one is not

            //I am going to setup a few boolean variables to determine which case I am in
            bool success = false;
            try
            {
                var noChange = oldTicket.AssignedToUserId == newTicket.AssignedToUserId;
                var assignment = string.IsNullOrEmpty(oldTicket.AssignedToUserId);
                var unassignment = string.IsNullOrEmpty(newTicket.AssignedToUserId);

                if (noChange)
                    return false;

                if (assignment)
                    success = await GenerateAssignmentNotification(oldTicket, newTicket);
                else if (unassignment)
                    success = await GenerateUnAssignmentNotification(oldTicket, newTicket);

                else
                {
                    success = await GenerateAssignmentNotification(oldTicket, newTicket);
                    success = await GenerateUnAssignmentNotification(oldTicket, newTicket);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }
            return success;
        }
        private static async Task<bool> GenerateUnAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            bool success = false;
            try
            {
                var notification = new TicketNotification
                {
                    Created = DateTime.Now,
                    Subject = $"You were unassigned from Ticket Id {newTicket.Id} on {DateTime.Now}",
                    Read = false,
                    RecipientId = oldTicket.AssignedToUserId,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    NotificationBody = $"Please acknowledge receipt of this notification by marking it read.",
                    TicketId = newTicket.Id


                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
                success = await EmailNotification(notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }
            return success;
        }
        private static async Task<bool> GenerateAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            bool success = false;
            try
            {
                var notification = new TicketNotification
                {
                    Created = DateTime.Now,
                    Subject = $"You were assigned to Ticket Id {newTicket.Id} on {DateTime.Now}",
                    Read = false,
                    RecipientId = oldTicket.AssignedToUserId,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    NotificationBody = $"Please acknowledge receipt of this notification by marking it read.",
                    TicketId = newTicket.Id

                };
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
                success = await EmailNotification(notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                success = false;
            }
            return success;
        }
        private static async Task<bool> EmailNotification(TicketNotification notification)
        {
            bool success = false;
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(notification.RecipientId);
                msg.Body = "<div style='white-space:pre-line'>"
                    + notification.NotificationBody
                    + "</div>"
                    + Environment.NewLine
                    + "Please click the <a href =\"" + _callbackUrl + "\">NEW TICKET</a> to view the details.";

                msg.Destination = user.Email;
                msg.Subject = notification.Subject;

                await ems.SendAsync(msg);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
                success = false;
            }
            return success;
        }
        public static int GetNewUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId && !t.Read).Count();
        }

        public static int GetAllUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId).Count();
        }

        public static List<TicketNotification> GetUnreadUserNotifications()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return db.TicketNotifications.Where(t => t.RecipientId == userId && !t.Read).ToList();
        }
    }
}