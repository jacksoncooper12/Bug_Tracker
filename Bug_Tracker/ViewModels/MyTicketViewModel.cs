using Bug_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bug_Tracker.ViewModels
{
    public class MyTicketViewModel
    {
        public List<Ticket> AllTickets { get; set; }
        public List<Ticket> MyTickets { get; set; }
    }
}