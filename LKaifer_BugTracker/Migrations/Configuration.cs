namespace LKaifer_BugTracker.Migrations
{
    using LKaifer_BugTracker.Helpers;
    using LKaifer_BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {

            #region Role Manager
            var roleManager = new RoleManager<IdentityRole>(
                              new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }


            #endregion

            //I need to create a few users that will eventually occupy the roles of either Admin or Moderator

            var userManager = new UserManager<ApplicationUser>(
                               new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "lmkaifer@hotmail.com"))
            {
                userManager.Create(new ApplicationUser

                {
                    UserName = "lmkaifer@hotmail.com",
                    Email = "lmkaifer@hotmail.com",
                    FirstName = "Lisa",
                    LastName = "Kaifer",
                    DisplayName = "lmkaifer"
                }, "Coder@1234");
            }

            if (!context.Users.Any(u => u.Email == "Manager@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser

                {
                    UserName = "Manager@Mailinator.com",
                    Email = "Manager@Mailinator.com",
                    FirstName = "Josh",
                    LastName = "Fuller",
                    DisplayName = "Josh"
                }, "Coder@1234");
            }

            if (!context.Users.Any(u => u.Email == "Developer@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser

                {
                    UserName = "Developer@Mailinator.com",
                    Email = "Developer@Mailinator.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "JTwichell"
                }, "Coder@1234");
            }

            if (!context.Users.Any(u => u.Email == "Submitter@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser

                {
                    UserName = "Submitter@Mailinator.com",
                    Email = "Submitter@Mailinator.com",
                    FirstName = "Kimberlee",
                    LastName = "Brown",
                    DisplayName = "KBrown"
                }, "Coder@1234");
            }

            var adminId = userManager.FindByEmail("lmkaifer@hotmail.com").Id;
            userManager.AddToRole(adminId, "Admin");
            var pmId = userManager.FindByEmail("Manager@Mailinator.com").Id;
            userManager.AddToRole(pmId, "Manager");

            var devId = userManager.FindByEmail("Developer@Mailinator.com").Id;
            userManager.AddToRole(devId, "Developer");

            var subId = userManager.FindByEmail("Submitter@Mailinator.com").Id;
            userManager.AddToRole(subId, "Submitter");


            context.Projects.AddOrUpdate(
                p => p.Name,
                new Project { Name = "LKaifer Blog", Description = "This is the blog created by Lisa Kaifer.", Created = DateTime.Now },
                new Project { Name = "LKaifer Portfolio", Description = "This is Lisa Kaifer's portfolio.", Created = DateTime.Now },
                new Project { Name = "Bugs Be Gone", Description = "This is Lisa Kaifer's Bug Tracker.", Created = DateTime.Now}

                );

            #region Project Assignment
            var blogProjectId = context.Projects.FirstOrDefault(p => p.Name == "Spock IT Blog").Id;
            var bugTrackerProjectId = context.Projects.FirstOrDefault(p => p.Name == "Spock BugTracker").Id;

            var projectHelper = new ProjectsHelper();

            //Assign all three users to the Blog project
            projectHelper.AddUserToProject(pmId, blogProjectId);
            projectHelper.AddUserToProject(devId, blogProjectId);
            projectHelper.AddUserToProject(subId, blogProjectId);

            //Assign all three users to the Blog project
            projectHelper.AddUserToProject(pmId, bugTrackerProjectId);
            projectHelper.AddUserToProject(devId, bugTrackerProjectId);
            projectHelper.AddUserToProject(subId, bugTrackerProjectId);
            #endregion

            #region Priority, Status & Type creation (required FK's for a Ticket)
            context.TicketPriorities.AddOrUpdate(
                t => t.Name,
                new TicketPriority { Name = "Immediate", Description = "Highest priority" },
                new TicketPriority { Name = "High", Description = "A high priority" },
                new TicketPriority { Name = "Medium", Description = "A mid-level priority" },
                new TicketPriority { Name = "Low", Description = "A low prioirty" },
                new TicketPriority { Name = "None", Description = "No priority" }
                );

            context.TicketStatuses.AddOrUpdate(
                t => t.Name,
                new TicketStatus { Name = "New/Unassigned", Description = "An unassigned ticket" },
                new TicketStatus { Name = "Assigned", Description = "An assigned ticket" },
                new TicketStatus { Name = "In Progress", Description = "A ticket that is in progress"},
                new TicketStatus { Name = "Completed", Description = "A ticket that has been completed"},
                new TicketStatus { Name = "Archived", Description = "A ticket that has been archived."}
                );

            context.TicketTypes.AddOrUpdate(
                t => t.Name,
                new TicketType { Name = "Bug", Description = "A web application issue related to a feature not working as it should." },
                new TicketType { Name = "Documentation Request", Description = "A need for additional or updated documentation regarding web application." },
                new TicketType { Name = "New Function Request", Description = "A request for additional new functionality" },
                new TicketType { Name = "Update request", Description = "A request to update material or links to current material or links" }
                );

            context.SaveChanges();
            #endregion

            #region Ticket Creation
            context.Tickets.AddOrUpdate(
                p => p.Title,
                 //1 unassigned Bug on the Blog project
                 //1 assigned Defect on the Blog project
                 new Ticket
                 {
                     ProjectId = blogProjectId,
                     OwnerUserId = subId,
                     Title = "Seeded Ticket #1",
                     Description = "Testing a seeded Ticket",
                     Created = DateTime.Now,
                     TicketPriorityId = context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id,
                     TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.Name == "New / UnAssigned").Id,
                     TicketTypeId = context.TicketTypes.FirstOrDefault(t => t.Name == "Bug").Id,
                 },
                 new Ticket
                 {
                     ProjectId = blogProjectId,
                     OwnerUserId = subId,
                     AssignedToUserId = devId,
                     Title = "Seeded Ticket #2",
                     Description = "Testing a seeded Ticket",
                     Created = DateTime.Now,
                     TicketPriorityId = context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id,
                     TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.Name == "New / Assigned").Id,
                     TicketTypeId = context.TicketTypes.FirstOrDefault(t => t.Name == "Defect").Id,
                 },

                 //1 unassigned Bug on the BugTracker
                 //1 assigned Defect on the BugTracker
                 //1 unassigned Bug on the Blog project
                 //1 assigned Defect on the Blog project
                 new Ticket
                 {
                     ProjectId = bugTrackerProjectId,
                     OwnerUserId = subId,
                     Title = "Seeded Ticket #3",
                     Description = "Testing a seeded Ticket",
                     Created = DateTime.Now,
                     TicketPriorityId = context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id,
                     TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.Name == "New / UnAssigned").Id,
                     TicketTypeId = context.TicketTypes.FirstOrDefault(t => t.Name == "Bug").Id,
                 },
                 new Ticket
                 {
                     ProjectId = bugTrackerProjectId,
                     OwnerUserId = subId,
                     AssignedToUserId = devId,
                     Title = "Seeded Ticket #4",
                     Description = "Testing a seeded Ticket",
                     Created = DateTime.Now,
                     TicketPriorityId = context.TicketPriorities.FirstOrDefault(t => t.Name == "Medium").Id,
                     TicketStatusId = context.TicketStatuses.FirstOrDefault(t => t.Name == "New / Assigned").Id,
                     TicketTypeId = context.TicketTypes.FirstOrDefault(t => t.Name == "Defect").Id,
                 });
            #endregion
        }

        //You can use the DvSet<T>.AddOrUpdate() helper extension method
        //to avoid creating duplicate seed data

        //context.TicketStatuses.AddOrUpdate(
        //   t => t.Name,
        //   new TicketStatus { Name = "New/Unassigned" }
        //    );

        //  This method will be called after migrating to the latest version.

        //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
        //  to avoid creating duplicate seed data.
    }
    }

