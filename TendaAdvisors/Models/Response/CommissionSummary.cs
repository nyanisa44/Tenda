using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class CommissionSummary : BasicApplicationResponse
    {
        public int? Advisor_Id { get; set; }
        public int? AdvisorStatus_id { get; set; }
        public ContactResponse Client { get; set; } 
        public AdvisorResponse Advisor { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorSurName { get; set; }
        public string AdvisorIdnumber { get; set; }
        public string AdvisorMemberId { get; set; }
        public string AdvisorBranchName { get; set; }
        public string AdvisorBranchCode { get; set; }
        public string AdvisorAccountNumber { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}