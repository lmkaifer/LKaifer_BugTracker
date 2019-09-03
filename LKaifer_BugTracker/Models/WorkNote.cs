using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Models
{
    public class WorkNote
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}