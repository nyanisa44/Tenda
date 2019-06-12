using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class ApplicationReportResponse
    {
        public DateTime? Month { get; set; }
        public DateTime? DateRecieved { get; set; }
        public string AdvisorFirstName { get; set; }
        public string AdvisorLastName { get; set; }
        public string ApplicationType { get; set; }
        public string Supplier { get; set; }
        public string BenifitOption { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdNumber { get; set; }
        public string MemberNumber { get; set; }
        public DateTime? EnrolmentDate { get; set; }
        public bool AdvisorAppointment { get; set; }
        public bool ApplicationDoc { get; set; }
        public bool AdviceRecord { get; set; }
        public bool DisclosureDoc { get; set; }
        public string Status { get; set; }
    }
}