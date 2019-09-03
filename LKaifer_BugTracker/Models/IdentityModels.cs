﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using LKaifer_BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LKaifer_BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [StringLength(25, ErrorMessage = "First Name must be between 1 and 25 characters long.", MinimumLength = 1)]
        public string FirstName { get; set; }
        [StringLength(25, ErrorMessage = "Last Name must be between 1 and 25 characters long.", MinimumLength = 1)]

        public string LastName { get; set; }
        [StringLength(25, ErrorMessage = "Display Name must be between 1 and 25 characters long.", MinimumLength = 1)]

        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }

        }
        [NotMapped]
        public string FullNameWithEmail
        {
            get
            {
                return $"{LastName}, {FirstName}- {Email}";
            }
            
        }

        //Virtual Navigation Section
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<WorkNote> WorkNotes { get; set; }

        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketComments = new HashSet<TicketComment>();
            TicketHistories = new HashSet<TicketHistory>();
            WorkNotes = new HashSet<WorkNote>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
    }
    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketAttachment> TicketAttachments { get; set; }
    public DbSet<TicketHistory> TicketHistories { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketNotification> TicketNotifications { get; set; }
    public DbSet<TicketPriority> TicketPriorities { get; set; }
    public DbSet<TicketStatus> TicketStatuses { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<WorkNote> WorkNotes { get; set; }
}