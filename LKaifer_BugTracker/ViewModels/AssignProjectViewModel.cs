using LKaifer_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LKaifer_BugTracker.ViewModels
{
    public class AssignProjectViewModel
    {
        public ApplicationUser PersonWhoWillBeAssignedtoProject { get; set; }
        public SelectList ProjectList { get; set; }

        public string UserId { get; set; }

        public string SelectedProject { get; set; }
    }
}