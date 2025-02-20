﻿using Bug_Tracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Bug_Tracker.Helpers
{
    public class UserHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string GetFullName(string userId)
        {
            if(userId == null)
            {
                return ("Hello");
            }
            var user = db.Users.Find(userId);
            return user.FullName;
        }
        public string GetUserRole()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var roleId = user.Roles.Where(u => u.UserId == userId);
            return null;
        }
        public string GetUserRoleDos(string userId)
        {
            var user = db.Users.Find(userId);
            var roleId = user.Roles.FirstOrDefault();
            return roleId.ToString();
        }
        public string GetAvatarPath()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (user != null)
            {
                return user.AvatarPath;
            }
            else
            {
                return null;
            }
        }
    }
}