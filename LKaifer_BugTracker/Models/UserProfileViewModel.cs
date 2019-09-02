using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LKaifer_BugTracker.Models
{
    public class UserProfileViewModel
    {
        [StringLength(25, ErrorMessage = "First Name must be between 2 and 25 characters long.", MinimumLength = 2)]

        public string FirstName { get; set; }
        [StringLength(25, ErrorMessage = "Last Name must be between 2 and 25 characters long.", MinimumLength = 2)]
        public string LastName { get; set; }
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}