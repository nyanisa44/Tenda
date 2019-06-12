using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class BankNames
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Advisor> Advisors { get; set; }
    }
}