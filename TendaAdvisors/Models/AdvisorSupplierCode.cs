using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class AdvisorSupplierCode
    {   
        [Key]
        [Column(Order = 1)]
        public int SupplierId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int AdvisorId { get; set; }
        public string AdvisorCode { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual Advisor Advisor { get; set; }

    }
}