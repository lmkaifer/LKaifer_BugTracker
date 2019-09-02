using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]

    public class Ticket
    {
        public int Id { get; set; }

        [Display(Name = "Ticket Name")]
        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }
        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }
        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }
        [Display(Name = "Submitter")]
        public string OwnerUserId { get; set; }
        [Display(Name = "Developer")]
        public string AssignedToUserId { get; set; }

        [StringLength(100, ErrorMessage = "Title must be between 5 and 100 characters long.", MinimumLength = 5)]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description must be between 5 and 1000 characters long.", MinimumLength = 5)]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        
        //Virtual Navigation Section

        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }

        public virtual ICollection<TicketHistory> TicketHistories { get; set; }

        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketNotifications = new HashSet<TicketNotification>();
        }




    }
}
