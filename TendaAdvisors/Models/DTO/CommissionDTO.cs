using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.DTO
{
    public class CommissionDTO
    {
        public Advisor Advisor { get; set; }
        public int Count { get; set; }
        public List<CommissionStatement> CommissionList { get; set; }

    }
}