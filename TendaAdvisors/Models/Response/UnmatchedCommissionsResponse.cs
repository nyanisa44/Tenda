using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class UnmatchedCommissionsResponse
    {
        public int Id { get; set; }
        public string AdvisorName { get; set; }
        public string Surname { get; set; }
        public string Initial { get; set; }
        public string MemberNumber { get; set; }
        public string supplierName { get; set; }
        public string Reason { get; set; }
      
    }
}