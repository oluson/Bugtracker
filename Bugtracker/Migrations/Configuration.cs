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
            }
        }
    }
}


