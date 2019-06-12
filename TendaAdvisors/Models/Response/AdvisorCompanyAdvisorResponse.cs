using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorCompanyAdvisorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public int? Advisor_Id { get; set; }
        public int? AdvisorType_Id { get; set; }
    }
}