namespace Bug_Tracker.Migrations
{
    using Bug_Tracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bug_Tracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Bug_Tracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "jacksoncooper12@gmail.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "jacksoncooper12@gmail.com",
                    UserName = "jacksoncooper12@gmail.com",
                    FirstName = "Jackson",
                    LastName = "Cooper",

                }, "Coopdawg12!");
                //Step1 : Grab the Id that was just created by adding the above user
                var userId = userManager.FindByEmail("jacksoncooper12@gmail.com").Id;
                //Assing the created user with a secific role
                userManager.AddToRole(userId, "Admin");
            }
            if (!context.Users.Any(u => u.Email == "AndrewRussell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "AndrewRussell@coderfoundry.com",
                    UserName = "AndrewRussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",

                }, "The$tache15"); //password
                //Step1 : Grab the Id that was just created by adding the above user
                var userId = userManager.FindByEmail("AndrewRussell@coderfoundry.com").Id;
                //Assing the created user with a secific role
                userManager.AddToRole(userId, "ProjectManager");
            }
            if (!context.Users.Any(u => u.Email == "developer@mailinator.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "developer@mailinator.com",
                    UserName = "developer@mailinator.com",
                    FirstName = "Dev",
                    LastName = "Tester",

                }, "Coopdawg12!"); //password
                //Step1 : Grab the Id that was just created by adding the above user
                var userId = userManager.FindByEmail("developer@mailinator.com").Id;
                //Assing the created user with a secific role
                userManager.AddToRole(userId, "Developer");
            }
            if (!context.Users.Any(u => u.Email == "Submitter@mailinator.com"))
            {
                userManager.Create(new ApplicationUser()
                {
                    Email = "Submitter@mailinator.com",
                    UserName = "Submitter@mailinator.com",
                    FirstName = "Submitter",
                    LastName = "Tester",

                }, "Coopdawg12!"); //password
                //Step1 : Grab the Id that was just created by adding the above user
                var userId = userManager.FindByEmail("Submitter@mailinator.com").Id;
                //Assing the created user with a secific role
                userManager.AddToRole(userId, "Submitter");
            }
        }
    }
}
