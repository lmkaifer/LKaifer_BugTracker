using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        [StringLength(100, ErrorMessage = "Title must be between 5 and 100 characters long.", MinimumLength = 5)]

        public string Title { get; set; }
        [StringLength(1000, ErrorMessage = "Description must be between 5 and 1000 characters long.", MinimumLength = 5)]


        public string Description { get; set; }
        public string AttachmentUrl { get; set; }
        public DateTime Created { get; set; }
                

        //Virtual Navigation Section
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        

        
    }

}