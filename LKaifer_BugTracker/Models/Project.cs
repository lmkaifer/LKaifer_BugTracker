using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LKaifer_BugTracker.Models
{
    [Authorize]
    public class Project
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "Name must be between 5 and 100 characters long.", MinimumLength = 5)]
        public string Name { get; set; }
        [StringLength(1000, ErrorMessage = "Description must be between 5 and 1000 characters long.", MinimumLength = 5)]
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