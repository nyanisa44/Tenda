using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class ApplicationAdvisorHistory
    {
        public int Id { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime? DateEnded { get; set; }

        // Foreign Keys
        public int Application_Id { get; set; }
        public int Old_Advisor { get; set; }
        public int New_Advisor { get; set; }

        //Nav
        [ForeignKey("Old_Advisor")]
        public Advisor Advisor { get; set; }

        [ForeignKey("New_Advisor")]
        public Advisor Client { get; set; }

        [ForeignKey("Application_Id")]
        public Application Application { get; set; }

    }
}