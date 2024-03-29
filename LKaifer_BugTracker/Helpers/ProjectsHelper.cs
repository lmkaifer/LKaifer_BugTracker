﻿using LKaifer_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace LKaifer_BugTracker.Helpers
{

   
    public class ProjectsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        public List<string> UsersInRoleOnProject(int projectId, string roleName)
        {
            var people = new List<string>();

            foreach (var user in UsersOnProject(projectId).ToList())
            {
                if (roleHelper.IsUserInRole(user.Id, roleName))
                {
                    people.Add(user.Id);
                }
            }
            return people;
        }

        //public List<string> UsersNotInRoleOnProject(int projectId, string RoleName)
        //{
        //    var
        //}

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }
        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }
        public void AddUserToProject(string userId, int projectId)
        {
            try
            {
                if (!IsUserOnProject(userId, projectId))
                {
                    Project proj = db.Projects.Find(projectId);
                    var newUser = db.Users.Find(userId);

                    proj.Users.Add(newUser);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var cEvent in e.EntityValidationErrors)
                {
                    string errorMsg = "Entity of type" + cEvent.Entry.Entity.GetType().Name + " in state " + cEvent.Entry.State + " has the following validation errors:";
                    foreach (var ve in cEvent.ValidationErrors)
                    {
                        string childErrorMsg = "- Property: "+ ve.PropertyName + ", Error: " + ve.ErrorMessage;
                    }
                }
                throw;
            }
        }
        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified;//just saves this obj instance.
            }
        }

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }
        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }

        public ICollection<Project> ProjectsMissingRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var badProjects = new List<Project>();
            var allProjects = db.Projects.ToList();
            foreach (var project in allProjects)
            {
                if (UsersInRoleOnProject(project.Id, "Manager").Count() == 0 ||
                   UsersInRoleOnProject(project.Id, "Developer").Count() == 0 ||
                   UsersInRoleOnProject(project.Id, "Submitter").Count() == 0)
            {
                    badProjects.Add(project);
                }
            }
            return badProjects;

        }

        




    }



}
