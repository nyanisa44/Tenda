using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        public bool? Deleted { get; set; }
        public int MemberGroupCode { get; set; }

        public int IDNumberColumn { get; set; }
        public string IDNumberColumnName { get; set; }
        public int PolicyNumberColumn { get; set; }
        public string PolicyNumberColumnName { get; set; }
        public int AdvisorNameColumn { get; set; }
        public string AdvisorNameColumnName { get; set; }
        public int AdvisorSurnameColumn { get; set; }
        public string AdvisorSurnameColumnName { get; set; }
        public int MemberNumberColumn { get; set; }
        public string MemberNumberColumnName { get; set; }
        public int SurnameColumn { get; set; }
        public string SurnameColumnName { get; set; }
        public int InitialColumn { get; set; }
        public string InitialColumnName { get; set; }
        public int EnrollmentDateColumn { get; set; }
        public string EnrollmentDateColumnName { get; set; }
        public int TerminationDateColumn { get; set; }
        public string TerminationDateColumnName { get; set; }
        public int TransactionDateColumn { get; set; }
        public string TransactionDateColumnName { get; set; }
        public int SubscriptionDueColumn { get; set; }
        public string SubscriptionDueColumnName { get; set; }
        public int SubscriptionReceivedColumn { get; set; }
        public string SubscriptionReceivedColumnName { get; set; }

        public int CommissionExclVatColumn { get; set; }
        public string CommissionExclVatColumnName { get; set; }
        public int CommissionInclVatColumn { get; set; }
        public string CommissionInclVatColumnName { get; set; }
        public int? MainContact_Id { get; set; }
        //Nav
        [ForeignKey("MainContact_Id")]
        public Contact MainContact { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<LicenseType> Licenses { get; set; }
        public virtual ICollection<AdvisorSupplierCode> AdvisorSupplierCodes { get; set; }
        public virtual ICollection<ApplicationSupplier> ApplicationSuplliers { get; set; }
    }
}