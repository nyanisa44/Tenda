using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class SupplierLicenseTypes
    {
        [Key]
        [Column(Order = 1)]
        public int Supplier_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public int LicenseType_Id { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual LicenseType LicenseType { get; set; }
    }
}