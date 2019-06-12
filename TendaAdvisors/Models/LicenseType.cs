using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class LicenseType
    {
        public int Id { get; set; }
        public int LicenseCategoryId { get; set; }
        [MaxLength(2)]
        public string SubCategory { get; set; }
        [MaxLength(128)]
        public string Description { get; set; }
        public ICollection<Advisor> Advisors { get; set; }
        public ICollection<Supplier> Suppliers { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}


