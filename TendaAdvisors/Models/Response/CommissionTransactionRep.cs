using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class CommissionTransactionRep
    {
        public int SupplierId { get; set; }
        public string Client { get; set; }
        public string ClientIdNumber { get; set; }
        public DateTime? Date { get; set; }
        public string Supplier { get; set; }
        public string ApplicationType { get; set; }
        public decimal? Comm { get; set; }
    }
}