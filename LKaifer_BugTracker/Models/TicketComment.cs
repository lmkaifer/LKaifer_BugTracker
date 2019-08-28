using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]
    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }

        [StringLength(200, ErrorMessage = "Comment must be between 5 and 200 characters long.", MinimumLength = 5)]

        public string CommentBody { get; set; }
        public DateTime Created { get; set; }

        //Virtual Navigation Section
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }
       
    }
}