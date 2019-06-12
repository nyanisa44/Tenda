using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class QueryResponse
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public bool Deleted { get; set; }
        public int? QueryType_Id { get; set; }
        public int? Advisor_Id { get; set; }
        public int? Application_Id { get; set; }
        public QueryType QueryType { get; set; }
        public string QueryName { get; internal set; }
    }
}