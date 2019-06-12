using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [MaxLength(50)]
       // public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Initial { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        //[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Tel3 { get; set; }

        public string TelWork1 { get; set; }
        public string TelWork2 { get; set; }
        public string TelWork3 { get; set; }
        //[Required]
        public string Cell1 { get; set; }
        public string Cell2 { get; set; }
        public string Cell3 { get; set; }
        //[Required]
        public DateTime? BirthDate { get; set; }
        public DateTime? SchemeRegisterDate { get; set; }
        public DateTime? SchemeTerminatedDate { get; set; }
        public DateTime? LinkDate { get; set; }
        public string IdNumber { get; set; }
        public string GroupCode { get; set; }
        public string JobTitle { get; set; }
        public string EmployerName { get; set; }
        public string Language { get; set; }
        public string Network { get; set; }
        public string BenefitCode { get; set; }
        public string SalaryCode { get; set; }
        public string SalaryScale { get; set; }
        [DataType(DataType.Currency)]
        public decimal? MonthlyFeeCurrent { get; set; }
        public string BusinessUnit { get; set; }
        [MaxLength(255)]
        public string photoUrl { get; set; }
        public bool? Upsell { get; set; }
        public bool? ChronicPMB { get; set; }
        public int? DependantsAdult { get; set; }
        public int? DependantsChild { get; set; }
        public bool? Deleted { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public int? ContactType_Id { get; set; }
        public int? ContactTitle_Id { get; set; }
        //Nav
        [ForeignKey("ContactType_Id")]
        public ContactType ContactType { get; set; }
        [ForeignKey("ContactTitle_Id")]
        public ContactTitles ContactTitle { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}