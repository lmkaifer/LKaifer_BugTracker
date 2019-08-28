using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]
    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "Description must be between 5 and 200 characters long.", MinimumLength = 5)]

        public string Description { get; set; }

        //Virtual Navigation Section
        public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketType()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}