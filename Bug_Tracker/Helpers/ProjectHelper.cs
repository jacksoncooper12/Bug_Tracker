using Bug_Tracker.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Helpers
{
    public class ProjectHelper
    {
        //new access to the database
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        public bool IsUserOnProject(string userId, int projectId)
        {
            Project project = db.Projects.Find(projectId);
            return project.Users.Any(u => u.Id == userId);
        }
        //add one or more users to a project
        public void AddUserToProject(string userId, int projectId)
        {
            if(!IsUserOnProject(userId, projectId))
            {
                Project project = db.Projects.Find(projectId);
                var user = db.Users.Find(userId);
                project.Users.Add(user);
                db.SaveChanges();
            }

        }
        public List<ApplicationUser> ListUsersOnProjectInRole(int projectId, string roleName)
        {
            var userList = ListUsersOnProject(projectId);
            var resultList = new List<ApplicationUser>();
            foreach(var user in userList)
            {
                if(roleHelper.IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
        public List<Project> ListUserProjects(string userId)
        {
            if (roleHelper.IsUserInRole(userId, "Admin"))
            {
                var projects2 = db.Projects.ToList();
                return projects2;
            }
            var user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return projects;
        }
        //remove one or more users from a project 
        public bool RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            var result = project.Users.Remove(user);
            db.SaveChanges();

            return result;
        }
        //list users on project
        public ICollection<ApplicationUser> ListUsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }
        // list users not on project
        public List<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var resultList = new List<ApplicationUser>();
            foreach (var user in db.Users.ToList())
            {
                if (!IsUserOnProject(user.Id, projectId))
                {
                    resultList.Add(user);

                }
            }
            return resultList;
        }
        public List<Project> ListProjects()
        {
            var projects = db.Projects.ToList();
            return projects;
        }
        //find users not on project for use in ListUsersNotOnProject



        //optional (do it): list users on a project that occupy specific roles


    }
}