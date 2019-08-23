using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{
    public class CommonHelper
    {
        protected static ApplicationDbContext db = new ApplicationDbContext();
        protected static UserRolesHelper roleHelper = new UserRolesHelper();
        protected static ProjectsHelper projectHelper = new ProjectsHelper();

    }
}