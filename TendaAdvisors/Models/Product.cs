using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class Product
    {
        public int Id { get; set; }
        //[Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        
   

        //FK
        public int LicenseTypeId { get; set; }
        //Nav
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public LicenseType LicenseType { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
    }
}