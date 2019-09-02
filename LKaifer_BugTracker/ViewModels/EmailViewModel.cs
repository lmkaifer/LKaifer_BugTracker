using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.ViewModels
{
    public class EmailViewModel
    {
        [Required]
        public string ToUserId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
    }
}