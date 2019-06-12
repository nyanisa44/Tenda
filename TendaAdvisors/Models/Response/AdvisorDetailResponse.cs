using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorDetailResponse
    {
        // ADVISOR FIELDS
        public int AdvisorId { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public double Allowance { get; set; }
        public decimal TaxDirectiveRate { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        // ADVISOR FIELDS

        // ADVISOR STATUS FIELDS
        public string AdvisorStatusName { get; set; }
        // ADVISOR STATUS FIELDS

        // ADVISOR TYPE FIELDS
        public int AdvisorTypeId { get; set; }
        public string AdvisorTypeTitle { get; set; }
        // ADVISOR TYPE FIELDS

        // ADVISOR BANK FIELDS
        public string BankName { get; set; }
        public string BranchCodeName { get; set; }
        // ADVISOR BANK FIELDS

        // ADVISOR COMPANY FIELDS
        public string CompanyName { get; set; }
        public string CompanyImageUrl { get; set; }
        public string CompanyVatNumber { get; set; }
        // ADVISOR COMPANY FIELDS

        // ADVISOR CONTACT FIELDS
        public string IdNumber { get; set; }
        public string PhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string Cell1 { get; set; }
        public string Cell2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string ContactTitleName { get; set; }
        // ADVISOR CONTACT FIELDS

        //Address (FistOrDefault)
        public int ContactId { get; set; }
        public string AddressType_name { get; set; }
        public string AddressDescription { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Suburb { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province_name { get; set; }
        public string Country_name { get; set; }
        public string MapUrl { get; set; }

        public string CmsCode { get; set; }

        // ADVISOR USER FIELDS
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        // ADVISOR USER FIELDS

        // ADVISOR DOCUMENTS FIELDS
        public IEnumerable<int> DocumentIds { get; set; }
        public IEnumerable<int> DocumentTypeIds { get; set; }
        // ADVISOR DOCUMENTS FIELDS
    }
}