namespace Bugtracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bugtracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Bugtracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            {
                var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
                if (!context.Roles.Any(r => r.Name == "Admin"))
                {
                    roleManager.Create(new IdentityRole { Name = "Admin" });
                }
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                if (!context.Users.Any(u => u.Email == "oosajere@gmail.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "oosajere@gmail.com",
                        Email = "oosajere@gmail.com",
                        FirstName = "Olu",
                        LastName = "Osajere",
                        DisplayName = "oosajere@gmail.com"
                    }, "Wilson#123");
                    var userId = userManager.FindByEmail("oosajere@gmail.com").Id;
                    userManager.AddToRole(userId, "Admin");
                }
                if (!context.Roles.Any(r => r.Name == "Project Manager"))
                {
                    roleManager.Create(new IdentityRole { Name = "Project Manager" });
                }
                if (!context.Users.Any(u => u.Email == "projectmanager@dispostable.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "projectmanager@dispostable.com",
                        Email = "projectmanager@dispostable.com",
                        FirstName = "Project",
                        LastName = "Manager",
                        DisplayName = "Project Manager"
                    }, "Pmanager#1");
                    var userId = userManager.FindByEmail("projectmanager@dispostable.com").Id;
                    userManager.AddToRole(userId, "Project Manager");
                }

                if (!context.Roles.Any(r => r.Name == "Project Manager 2"))
                {
                    roleManager.Create(new IdentityRole { Name = "Project Manager 2" });
                }
                if (!context.Users.Any(u => u.Email == "projectmanager2@dispostable.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "projectmanager2@dispostable.com",
                        Email = "projectmanager2@dispostable.com",
                        FirstName = "Project2",
                        LastName = "Manager2",
                        DisplayName = "Project Manager2"
                    }, "Pmanager#2");
                    var userId = userManager.FindByEmail("projectmanager2@dispostable.com").Id;
                    userManager.AddToRole(userId, "Project Manager2");
                }
                if (!context.Roles.Any(r => r.Name == "Developer"))
                {
                    roleManager.Create(new IdentityRole { Name = "Developer" });
                }
                if (!context.Users.Any(u => u.Email == "developer@dispostable.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "developer@dispostable.com",
                        Email = "developer@dispostable.com",
                        FirstName = "Project",
                        LastName = "Developer",
                        DisplayName = "Developer"
                    }, "Developer#1");
                    var userId = userManager.FindByEmail("developer@dispostable.com").Id;
                    userManager.AddToRole(userId, "Developer");
                }
                if (!context.Roles.Any(r => r.Name == "Submitter"))
                {
                    roleManager.Create(new IdentityRole { Name = "Submitter" });
                }

                //TicketPriority Seeding Method
                if (!context.TicketPriority.Any(t => t.Name == "Low"))
                {
                    var ticketPriorityLow = new TicketPriorities();
                    ticketPriorityLow.Name = "Low";
                    context.TicketPriority.Add(ticketPriorityLow);
                }
                if (!context.TicketPriority.Any(t => t.Name == "Medium"))
                {
                    var ticketPriorityMedium = new TicketPriorities();
                    ticketPriorityMedium.Name = "Medium";
                    context.TicketPriority.Add(ticketPriorityMedium);
                }
                if (!context.TicketPriority.Any(t => t.Name == "High"))
                {
                    var ticketPriorityHigh = new TicketPriorities();
                    ticketPriorityHigh.Name = "High";
                    context.TicketPriority.Add(ticketPriorityHigh);
                }

               // TicketTypes Seeding Method
                if (!context.TicketType.Any(t => t.Name == "General Issue"))
                {
                    var ticketTypeGeneralIssue = new TicketTypes();
                    ticketTypeGeneralIssue.Name = "General Issue";
                    context.TicketType.Add(ticketTypeGeneralIssue);
                }
                if (!context.TicketType.Any(t => t.Name == "Feature Issue"))
                {
                    var ticketTypeFeatureIssue = new TicketTypes();
                    ticketTypeFeatureIssue.Name = "Feature Issue";
                    context.TicketType.Add(ticketTypeFeatureIssue);
                }
                if (!context.TicketType.Any(t => t.Name == "Client Issue"))
                {
                    var ticketTypeClientIssue = new TicketTypes();
                    ticketTypeClientIssue.Name = "Client Issue";
                    context.TicketType.Add(ticketTypeClientIssue);
                }
                if (!context.TicketType.Any(t => t.Name == "Others"))
                {
                    var ticketTypeOthers = new TicketTypes();
                    ticketTypeOthers.Name = "Others";
                    context.TicketType.Add(ticketTypeOthers);
                }

                //TicketStatus Seeding Method

                if (!context.TicketStatus.Any(t => t.Name == "Unassigned"))
                {
                    var ticketStatusUnassigned = new TicketStatuses();
                    ticketStatusUnassigned.Name = "Unassigned";
                    context.TicketStatus.Add(ticketStatusUnassigned);
                }

                if (!context.TicketStatus.Any(t => t.Name == "Active/Assigned"))
                {
                    var ticketStatusActiveAssigned = new TicketStatuses();
                    ticketStatusActiveAssigned.Name = "Active/Assigned";
                    context.TicketStatus.Add(ticketStatusActiveAssigned);
                }

                if (!context.TicketStatus.Any(t => t.Name == "Resolved"))
                {
                    var ticketStatusResolved = new TicketStatuses();
                    ticketStatusResolved.Name = "Resolved";
                    context.TicketStatus.Add(ticketStatusResolved);
                }

                // //Ticket Notification Seeding Method
            //    context.TicketNotification.AddOrUpdate(n => n.UserId,
            //   new TicketNotification() { UserId = "Ticket Submitted" },
            //   new TicketNotification() { UserId = "Ticket Assigned" },
            //   new TicketNotification() { UserId = "Ticket Resolved" },
            //   new TicketNotification() { UserId = "Reminder: Update Tickets" },
            //   new TicketNotification() { UserId = "Ticket Modified" },
            //   new TicketNotification() { UserId = "Ticket Reassigned" },
            //   new TicketNotification() { UserId = "Project Reassigned" },
            //   new TicketNotification() { UserId = "Project Assigned" },
            //   new TicketNotification() { UserId = "New Project Manager" },
            //   new TicketNotification() { UserId = "Project Deadline Changed" }
            //   );
            }
        }
    }
}


