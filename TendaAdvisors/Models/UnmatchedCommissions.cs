﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class UnmatchedCommissions
    {
        public int Id { get; set; }
        public string MemberGroupCode { get; set; }
        public string AdvisorName { get; set; }
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
        public decimal? SubscriptionDue { get; set; }
        public decimal? SubscriptionReceived { get; set; }
        public decimal? CommissionInclVAT { get; set; }
        public decimal? CommissionExclVAT { get; set; }
        public decimal? AdvisorCommission { get; set; }
        public decimal? CompanyCommission { get; set; }
        public decimal? AdvisorTax { get; set; }
        public decimal? AdvisorTaxRate { get; set; }
        public string supplierName { get; set; }
        public string Reasons { get; set; }


        public int? SupplierId { get; set; }

        public int? ImportFileId { get; set; }


        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }

        [ForeignKey("ImportFileId")]
        public ImportFile ImportFile { get; set; }


    }
}