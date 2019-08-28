using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]
    public class Project
    {
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 5 and 50 characters long.", MinimumLength = 5)]

        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "Description must be between 5 and 300 characters long.", MinimumLength = 5)]

        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }

        public string OwnerUserId { get; set; }

        //virtual nav
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
        }
    }
}