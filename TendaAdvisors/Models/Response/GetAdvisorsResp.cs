using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class GetAdvisorsResp
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
    
        // Nav
        public int? ContactTitle_Id { get; set; }      

        public Contact Contact { get; set; }
    
        public AdvisorType AdvisorType { get; set; }

        public AdvisorStatus AdvisorStatus { get; set; }

    }
}