using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ApplicationSupplier
    {
        [Key]
        [Column(Order = 1)]
        public int ApplicationId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int SupplierId { get; set; }
        public string MemberNumber { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Application Application { get; set; }
    }
}