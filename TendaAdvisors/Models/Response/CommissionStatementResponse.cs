using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class CommissionStatementResponse
    {

        public int Id { get; set; }
        public string Surname { get; set; }
        public string Initial { get; set; }
        public string MemberNumber { get; set; }
        public string MemberSearchKey { get; set; }
        public DateTime? TerminationDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? AdvisorCommission { get; set; }
        public decimal? CompanyCommission { get; set; }
        public decimal? CommissionInclVAT { get; set; }
        public decimal? CommissionExclVAT { get; set; }
        public decimal? AdvisorTax { get; set; }
        public decimal? AdvisorTaxRate { get; set; }
        //public int CompanyId { get; set; } Can get this from Advisor
        public int? AdvisorId { get; set; }
        //May be null as not yet captured.
        public int? ClientId { get; set; }
        public int ProductId { get; set; }
        public string supplierName { get; set; }

        public string AdvisorName { get; set; }
        public string AdvisorLastname { get; set; }
    }
}