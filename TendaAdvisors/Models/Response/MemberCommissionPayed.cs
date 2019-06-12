using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class MemberCommissionPayed
    {
        public string ClientName { get; set; }
        public string ClientIdNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Supplier { get; set; }
        public string Adviser { get; set; }
        public double CommAmmount { get; set; }

    }
}