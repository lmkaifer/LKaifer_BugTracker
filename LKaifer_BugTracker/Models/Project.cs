using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
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