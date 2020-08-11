using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bug_Tracker.ViewModels
{
    public class ProjectWizardVM
    {
        [Required]
        public string Name { get; set; }
        public string ProjectManagerId { get; set; }
        public ICollection<string> DeveloperIds { get; set; }
        public ICollection<string> SubmitterIds { get; set; }
        public ProjectWizardVM()
        {
            DeveloperIds = new HashSet<string>();
            SubmitterIds = new HashSet<string>();
        }
    }


}