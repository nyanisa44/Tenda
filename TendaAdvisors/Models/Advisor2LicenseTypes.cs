using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    [Table("Advisor2LicenceType")]
    public class Advisor2LicenseTypes
    {
        [Key,Column(Order = 0)]
        public int AdvisorId { get; set; }
        [Key, Column(Order = 1)]
        public int LicenseTypeId { get; set; }

        public int Share { get; set; }
        public bool underSupervision { get; set; }

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