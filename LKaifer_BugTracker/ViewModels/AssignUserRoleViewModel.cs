using LKaifer_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LKaifer_BugTracker.ViewModels
{
    public class AssignUserRoleViewModel
    {
        public ApplicationUser PersonWhoWillBeAssignedtotheRole { get; set; }
        public SelectList RoleList { get; set; }
        public string UserId { get; set; }

        public string SelectedRole { get; set; }
    }
}