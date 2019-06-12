using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorLicenseResponse
    {
        public int AdvisorId { get; set; }
        public int SupervisorId { get; set; }
        public int LicenseTypeId { get; set; }
        public string LicenseType { get; set; }
        public double Share { get; set; }
        public DateTime ValidCommissionFromDate { get; set; }
        public DateTime? ValidCommissionToDate { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }
        public string Supplier { get; set; }
        public string Product { get; set; }
        public bool UnderSupervision { get; set; }
        public string SupervisorFirstName { get; set; }
        public string SupervisorLastName { get; set; }
    }
}