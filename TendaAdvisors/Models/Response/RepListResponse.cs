using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class RepListResponse
    {

        public int? SupervisorId { get; set; }
        public string SupervisorName { get; set; }
        public string SupervisorSurname { get; set; }

        public int AdviId { get; set; }
        public string AdviName { get; set; }
        public string Role { get; set; }
        public string AdviSurname { get; set; }
        public string AdviIdNumer { get; set; }
        public string Status { get; set; }

        public DateTime? SupEffFrom { get; set; }
        public DateTime? SupEffTo { get; set; }
        public DateTime? EffFrom { get; set; }
        public DateTime? EffTo { get; set; }

        public string AdviBrCode { get; set; }
        public bool? UnderSupervision { get; set; }
        public string LicenseType { get; set; }
    }
}