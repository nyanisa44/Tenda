using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class CommissionStatement
    {
        public int Id { get; set; }
        public string MemberGroupCode { get; set; }
        public string Surname { get; set; }
        public string Initial { get; set; }
        public string MemberNumber { get; set; }
        public string MemberSearchKey { get; set; }
        public string MemberSearchValue { get; set; }
        public DateTime? CommisionRunDate { get; set; }
        public string CommisionRunUser { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? TransactionDate { get; set; }

        public DateTime? ApprovalDateFrom { get; set; }
        public DateTime? ApprovalDateTo { get; set; }
        
        public string ApprovalStatus { get; set; }

        public decimal? SubscriptionDue { get; set; }
        public decimal? SubscriptionReceived { get; set; }
        public decimal? CommissionInclVAT { get; set; }
        public decimal? CommissionExclVAT { get; set; }
     
        public decimal? AdvisorCommission { get; set; }
        public decimal? CompanyCommission { get; set; }
        public decimal? AdvisorTax { get; set; }
        public decimal? AdvisorTaxRate { get; set; }

        //public int CompanyId { get; set; } Can get this from Advisor
        public int? AdvisorId { get; set; }
        //May be null as not yet captured.
        public int? ClientId { get; set; }
        public int ProductId { get; set; }

        public int? SupplierId { get; set; }

        public int? ImportFileId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [ForeignKey("ImportFileId")]
        public ImportFile ImportFile { get; set; }
    }
}