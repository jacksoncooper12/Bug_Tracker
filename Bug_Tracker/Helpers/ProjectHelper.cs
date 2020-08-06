using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Helpers
{
    public class ProjectHelper
    {
        //new access to the database
        private ApplicationDbContext db = new ApplicationDbContext();
        //add one or more users to a project
        public void AddUserToProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Add(user);
            return;
        }
        //remove one or more users from a project 
        public bool RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            var result = project.Users.Remove(user);
            return result;
        }
        //list users on project
        public List<ApplicationUser> ListUsersOnProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var resultList = new List<ApplicationUser>();
            resultList.AddRange(project.Users);
            return resultList;
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
        //find users not on project for use in ListUsersNotOnProject
        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            return project.Users.Contains(user);
        }



        //optional (do it): list users on a project that occupy specific roles


    }
}