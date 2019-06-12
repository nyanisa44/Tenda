using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Company
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string VatNumber { get; set; }
        public string FsbNumber { get; set; }
        public string ImageUrl { get; set; }

        //@TODO: How to limit this collection to only Advisor.IsKeyIndividual == true Custom Collection?
        //public ICollection<Advisor> KeyIndividuals { get; set; }

        public int? ContactDetails_Id { get; set; }
        [ForeignKey("ContactDetails_Id")]
        public Contact ContactDetails { get; set; }
        public ICollection<LicenseType> Licenses { get; set; }
        public ICollection<Advisor> Advisors { get; set; }
    }
}