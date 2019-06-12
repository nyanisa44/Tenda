using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TendaAdvisors.Models
{
    public class AdvisorStatus
    {
        public int Id { get; set; }
        //Invalid, Not Regustered
        [MaxLength(20)]
        public string Name { get; set; }

        //nav

        public ICollection<Advisor> Advisors { get; set; }
    }
}

