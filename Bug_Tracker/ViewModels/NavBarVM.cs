using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.ViewModels
{
    public class NavBarVM
    {
        public ICollection<Project> Projects { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}