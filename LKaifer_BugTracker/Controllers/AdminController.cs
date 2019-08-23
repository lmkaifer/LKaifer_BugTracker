using LKaifer_BugTracker.Helpers;
using LKaifer_BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Security;

namespace LKaifer_BugTracker.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller

    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();
        //Get: Admin

        public ActionResult UserIndex()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email
            }).ToList();

            return View(users);
        }
        public ActionResult UserProfileViewModel()
        {
            return View();

        }
        

        public ActionResult ManageUserRole(string userId)
        {
            //My code; not Jason's
            //How do I load up a DropDownList with Role Information?
            //new SelectList("The list of data pushed into the control",   
            //"the column that will be used to communicate my selection(s) to the post",
            //"the column we show the user inside the control",
            //"If they already occupy a role..show this instead of nothing")
            AssignUserRoleViewModel userVM = new AssignUserRoleViewModel();
            var currentRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            userVM.PersonWhoWillBeAssignedtotheRole = db.Users.Find(userId);
            userVM.RoleList = new SelectList(db.Roles.ToList(), "Name", "Name", currentRole);
            return View(userVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserRole(AssignUserRoleViewModel model)
        {   //My Code & not Jason's
            //This is where I will be using the UserRolesHelper class to make sure my user occupies the proper role
            //The first thing I want to do is make sure I remove the user from ALL ROLES they may occupy

            foreach (var role in roleHelper.ListUserRoles(model.UserId))
            {
                roleHelper.RemoveUserFromRole(model.UserId, role);
            }

            //If the incoming role selection is NOT NULL I want to assign the user to the selected role
            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                roleHelper.AddUserToRole(model.UserId, model.SelectedRole);

            }
            return RedirectToAction("UserIndex");
        }

        public ActionResult ManageRoles()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email

            }).ToList();

            ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullNameWithEmail");
            ViewBag.RoleName = new SelectList(db.Roles.ToList(), "Name", "Name");
            return View(users);
        }

        [HttpPost]
        public ActionResult ManageRoles(List<string> users, string roleName)
        {
            if (users != null)
            {
                //Let's iterate over the incoming list of Users that were selected from the form
                foreach (var userId in users)
                {

                    //and remove each of them from whatever role they occupy only to add them back to the selected role
                    foreach (var role in roleHelper.ListUserRoles(userId))
                    {
                        roleHelper.RemoveUserFromRole(userId, role);
                    }

                    if (!string.IsNullOrEmpty(roleName))
                    //Only to add them back to the selected role

                    {
                        roleHelper.AddUserToRole(userId, roleName);
                    }

                }
            }
            ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullNameWithEmail");
            ViewBag.RoleName = new SelectList(db.Roles.ToList(), "Name", "Name");
            return RedirectToAction("ManageRoles");

        }


        public ActionResult ManageUserProjects(string userId)
        {
            ViewBag.UserObject = db.Users.Find(userId);
            //I just need a list of projects that this user is on. . .
            var myProjects = projectHelper.ListUserProjects(userId).Select(p => p.Id);
            ViewBag.Projects = new MultiSelectList(db.Projects.ToList(), "Id", "Name", myProjects);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserProjects(List<int> projects, string userId)
        {
            //First let's remove the user from all projects
            foreach (var project in projectHelper.ListUserProjects(userId).ToList())
            {
                projectHelper.RemoveUserFromProject(userId, project.Id);
            }

            //Then if the incoming list is not null we will reassign them.
            if (projects != null)
            {
                foreach (var projectId in projects)
                {
                    projectHelper.AddUserToProject(userId, projectId);
                }
            }

            return RedirectToAction("Index", "Projects");
        }




       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectsUsers(int projectId, List<string> ProjectManagers, List<string> Developers, List<string> Submitters, List<string> Administrators)
        {
            //Step 1: Remove all users from the incoming project
            foreach (var user in projectHelper.UsersOnProject(projectId).ToList())
            {
                projectHelper.RemoveUserFromProject(user.Id, projectId);
            }
            //Step 2: Add back all the selected PM's
            if (ProjectManagers != null)
            {
                foreach (var projectManagerId in ProjectManagers)
                {
                    projectHelper.AddUserToProject(projectManagerId, projectId);
                }
            }
            if (Developers != null)
            {
                foreach (var developerId in Developers)
                {
                    projectHelper.AddUserToProject(developerId, projectId);
                }
            }
            if (Submitters != null)
            {
                foreach (var submitterId in Submitters)
                {
                    projectHelper.AddUserToProject(submitterId, projectId);
                }

            }
            if (Administrators != null)
            {
                foreach (var adminId in Administrators)
                {
                    projectHelper.AddUserToProject(adminId, projectId);
                }

            }
            return RedirectToAction("Index", "Projects");

        }

        //public ActionResult RemoveProjectUser
        //{

        //}

        //get
        //public ActionResult ManageProjects()
        //{
        //    var users = db.Users.Select(u => new UserProfileViewModel

        //    {
        //        Id = u.Id,
        //        FirstName = u.FirstName,
        //        LastName = u.LastName,
        //        Email = u.Email

        //    }).ToList();

        //    ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullNameWithEmail");
        //    ViewBag.projectIds = new SelectList(db.Projects.ToList(), "Id", "Name");
        //    return View(users);
        //}

        //[HttpPost]
        //public ActionResult ManageProjects(List<string> users, List<int> projectIds)
        //{
        //    if (users != null)
        //    {
        //        //Let's iterate over the incoming list of Users that were selected from the form
        //        foreach (var projectId in projects)
        //        {

        //            //and remove each of them from whatever project they occupy only to add them back to the selected project
        //            foreach (var user in projectHelper.UsersOnProject(projectId).ToList())
        //            {
        //                projectHelper.RemoveUserFromProject(user.Id, projectId);
        //            }

        //            if (!string.IsNullOrEmpty(roleName))
        //            //Only to add them back to the selected role

        //            {
        //                roleHelper.AddUserToRole(userId, roleName);
        //            }

        //        }
        //    }
        //    ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullNameWithEmail");
        //    ViewBag.RoleName = new SelectList(db.Roles.ToList(), "Name", "Name");
        //    return RedirectToAction("ManageRoles");

        //}

        //    [HttpGet]

        //    public ActionResult ManageUsers()
        //    {
        //        return View();
        //    }

        //}

        ////Get
        //public ActionResult ManageProjectUsers()
        //{
        //    var users = db.Users.Select(u => new UserProfileViewModel
        //    {
        //        Id = u.Id,
        //        FirstName = u.FirstName,
        //        LastName = u.LastName,
        //        Email = u.Email

        //    }).ToList();

        //    ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullNameWithEmail");
        //    ViewBag.RoleName = new SelectList(db.Roles.ToList(), "Name", "Name");
        //    return View(users);
        //}
        //}
    }
}
