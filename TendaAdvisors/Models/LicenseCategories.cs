using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class LicenseCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [MaxLength(60)]
        public string Name { get; set; }

        public ICollection<LicenseType> LicenseTypes { get; set; }
    }
}