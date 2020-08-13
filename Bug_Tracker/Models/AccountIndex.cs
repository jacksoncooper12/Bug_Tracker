using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.Models
{
    public class AccountIndex
    {
        public ApplicationUser User { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Project> AllProjects { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public string UserRole { get; set; }
        public AccountIndex()
        {
            User = new ApplicationUser();
            Projects = new HashSet<Project>();
            AllProjects = new HashSet<Project>();
            Tickets = new HashSet<Ticket>();
        }
    }
}