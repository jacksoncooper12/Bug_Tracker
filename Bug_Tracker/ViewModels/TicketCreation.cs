using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.ViewModels
{
    public class TicketCreation
    {
        public Project project { get; set; }
        public Ticket ticket { get; set; }
    }
}