using System;
using System.Collections.Generic;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorResponse
    {
        //Advisor
        public int Id { get; set; }
        public string AdvisorType { get; set; }
        public string FsbCode { get; set; }
        public string RegNumber { get; set; }
        public DateTime? DateAuthorized { get; set; }
        public string ComplianceOfficer { get; set; }
        public string ComplianceOfficerNumber { get; set; }
        public int AdvisorType_Id { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        //Contact
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public string MemberNumber { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Cell1 { get; set; }
        public string Cell2 { get; set; }

        //Address (FistOrDefault)
        public int AddressTyper_Id { get; set; }
        public string AddressType { get; set; }
        public string AddressDescription { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Suburb { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int Province_Id { get; set; }
        public string Province { get; set; }
        public int Country_id { get; set; }
        public string Country { get; set; }
        public string MapUrl { get; set; }
        
        // AdvisorDocuments
        public ICollection<AdvisorDocumentResponse> AdvisorDocuments { get; set; }

    }
}