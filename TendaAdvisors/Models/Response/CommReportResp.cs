using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class CommReportResp
    {
        public decimal? AdvisorCommision { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorLastname { get; set; }
        public DateTime? Date { get; set; }
    }
}