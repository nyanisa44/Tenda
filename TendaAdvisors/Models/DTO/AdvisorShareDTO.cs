using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models.DTO
{
    public class AdvisorShareDTO
    {
        public int AdvisorId { get; set; }
        public int LicenseTypeId { get; set; }
        public string LicenseTypeName { get; set; }
        public double Share { get; set; }
        public bool UnderSupervision { get; set; }
        public string supplier { get; set; }
        public string product { get; set; }

        public int Advisor { get; set; }

        [DataType(DataType.Date)]
        public DateTime? validCommissionFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? validCommissionToDate { get; set; }



        [DataType(DataType.Date)]
        public DateTime? ValidFromDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ValidToDate { get; set; }
    }
}