using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Advisor
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public bool? IsKeyIndividual { get; set; }
        public bool? Deleted { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastLoginDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        public string FsbCode { get; set; }
        public string CmsCode { get; set; }
        public string RegNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateAuthorized { get; set; }
        public string ComplianceOfficer { get; set; }
        public string ComplianceOffficerNumber { get; set; }
        public decimal TaxDirectiveRate { get; set; }

        public double Allowance { get; set; }

        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        public string AccountType { get; set; }
        public string AccountNumber { get; set; }

        //FK
        public int ContactId { get; set; }

        //User Account used for login
        public ViewModels.User User { get; set; }
        // Nav
        public int? ContactTitle_Id { get; set; }
        public int? Company_Id { get; set; }
        public int? AdvisorType_Id { get; set; }
        public int? AdvisorStatus_Id { get; set; }
        public int? BankName_Id { get; set; }
        public int? BranchCode_Id { get; set; }

        public Contact Contact { get; set; }
        [ForeignKey("ContactTitle_Id")]
        public ContactTitles ContactTitle { get; set; }
        [ForeignKey("Company_Id")]
        public Company Company { get; set; }
        [ForeignKey("AdvisorType_Id")]
        public AdvisorType AdvisorType { get; set; }
        [ForeignKey("AdvisorStatus_Id")]
        public AdvisorStatus AdvisorStatus { get; set; }
        [ForeignKey("BankName_Id")]
        public BankNames BankName { get; set; }
        [ForeignKey("BranchCode_Id")]
        public BankBranchCodes BranchCode { get; set; }
        public ICollection<Application> Applications { get; set; }
        public ICollection<LicenseType> Licenses { get; set; }
        public ICollection<AdvisorDocument> AdvisorDocuments { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Query> Querys { get; set; }

        //public virtual ICollection<AdvisorSupplierCode> AdvisorSupplierCodes { get; set; }


    }
}