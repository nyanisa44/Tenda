using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class AdvisorShareUnderSupervision
    {
        [Key, Column(Order = 0)]
        public int AdvisorId { get; set; }

        [Key, Column(Order = 1)]
        public int LicenseTypeId { get; set; }

        [Key, Column(Order = 2)]
        public string supplier { get; set; }

        [Key, Column(Order = 3)]
        public string product { get; set; }

        public double Share { get; set; }
        public bool underSupervision { get; set; }

        public int Advisor { get; set; }


        public int? SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }


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